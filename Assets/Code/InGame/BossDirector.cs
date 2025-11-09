using UnityEngine;

public class BossDirector : MonoBehaviour
{
    [Header("Refs")]
    public GameObject bossPrefab;
    public Transform spawnPoint;      // 없으면 자동: 화면 오른쪽 밖
    public Transform player;          // 없으면 자동 탐색

    [Header("Entrance")]
    public AudioClip warningSfx;
    public float entranceMoveTime = 2.0f;   // 보스가 자리로 들어오는 시간
    public Vector3 bossStopOffset = new Vector3(3f, 0f, 0f); // 화면 오른쪽 안쪽 위치
    public float playerNudgeX = -2.0f;      // 플레이어를 왼쪽으로 살짝 밀기
    public float playerNudgeTime = 0.8f;

    bool spawned;

    void Start()
    {
        if (!player)
        {
#if UNITY_2023_1_OR_NEWER
            var p = Object.FindFirstObjectByType<PlayerHealth>();
#else
            var p = Object.FindObjectOfType<PlayerHealth>();
#endif
            if (p) player = p.transform;
        }
    }

    void Update()
    {
        if (spawned) return;
        if (GameManager.Instance == null || !GameManager.Instance.IsBossTime) return;
        SpawnBoss();
        spawned = true;
    }

    void SpawnBoss()
    {
        if (!bossPrefab) { Debug.LogWarning("[BossDirector] bossPrefab 미지정"); return; }

        Vector3 pos = spawnPoint ? spawnPoint.position : GetOffscreenRight(3f);
        var go = Instantiate(bossPrefab, pos, Quaternion.identity);

        // 보스 컨트롤러에 타겟 전달
        var ctrl = go.GetComponent<BossController>();
        if (ctrl && player) ctrl.target = player;

        // 경고 사운드 + 화면 흔들림 + 연출 코루틴
        if (warningSfx) AudioSource.PlayClipAtPoint(warningSfx, Camera.main.transform.position, 0.8f);
        var shaker = Camera.main.GetComponent<ScreenShake2D>();
        shaker?.Shake(0.6f, 0.4f);

        StartCoroutine(Entrance(go));
    }

    System.Collections.IEnumerator Entrance(GameObject boss)
    {
        // 보스가 화면 안쪽 고정 위치로 부드럽게 이동
        var cam = Camera.main;
        Vector3 target = cam.transform.position;
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;
        target.x += halfW - bossStopOffset.x; // 오른쪽 안쪽
        target.y += bossStopOffset.y;
        target.z = 0f;

        Vector3 start = boss.transform.position;
        float t = 0f;
        while (t < entranceMoveTime)
        {
            t += Time.deltaTime;
            float k = Mathf.SmoothStep(0, 1, t / entranceMoveTime);
            boss.transform.position = Vector3.Lerp(start, target, k);
            yield return null;
        }

        // 플레이어를 살짝 왼쪽으로 밀며 회전 복원
        if (player)
        {
            var mover = player.GetComponent<PlayerMover>();
            if (mover) mover.NudgeLeftAndReset(playerNudgeX, playerNudgeTime);
            else StartCoroutine(SimpleNudge(player, playerNudgeX, playerNudgeTime));
        }

        // 보스 패턴 시작
        var ctrl = boss.GetComponent<BossController>();
        ctrl?.BeginPatterns();
    }

    System.Collections.IEnumerator SimpleNudge(Transform tr, float dx, float time)
    {
        Vector3 s = tr.position;
        Vector3 e = s + new Vector3(dx, 0, 0);
        float t = 0f;
        Quaternion r0 = tr.rotation;
        while (t < time)
        {
            t += Time.deltaTime;
            float k = Mathf.SmoothStep(0, 1, t / time);
            tr.position = Vector3.Lerp(s, e, k);
            tr.rotation = Quaternion.Slerp(r0, Quaternion.identity, k); // 회전 원복
            yield return null;
        }
        tr.position = e;
        tr.rotation = Quaternion.identity;
    }

    Vector3 GetOffscreenRight(float margin)
    {
        var cam = Camera.main;
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;
        Vector3 c = cam.transform.position;
        return new Vector3(c.x + halfW + margin, c.y, 0f);
    }
}
