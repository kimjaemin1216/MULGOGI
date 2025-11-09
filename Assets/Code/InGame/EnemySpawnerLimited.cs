using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerLimited : MonoBehaviour
{
    public enum SpawnSpace { OutsideCamera, RingAroundPlayer }

    [Header("Spawn Rules (Global)")]
    public SpawnSpace spawnSpace = SpawnSpace.OutsideCamera;
    public float cameraMargin = 2.5f;
    public float minDistanceFromPlayer = 5f;
    public int maxSpawnTries = 8;
    public float ringRadius = 12f;

    [Header("Stop Spawning After (sec)")]
    [SerializeField] float stopAfterSeconds = 60f;    // 60초 뒤 스폰 중단
    bool stopAll;

    [System.Serializable]
    public class Rule
    {
        public string typeId = "Enemy";
        public GameObject prefab;
        public int maxConcurrent = 3;
        public float interval = 2.0f;
        [HideInInspector] public float timer = 0f;
    }

    public Transform center;
    public Rule[] rules;

    // 내부 동시 수 제한(타입별)
    readonly Dictionary<string, int> liveCounts = new Dictionary<string, int>();

    void Awake()
    {
        if (center == null)
        {
#if UNITY_2023_1_OR_NEWER
            var p = Object.FindFirstObjectByType<PlayerController>();
#else
            var p = Object.FindObjectOfType<PlayerController>();
#endif
            if (p != null) center = p.transform;
        }
    }

    void Update()
    {
        if (center == null || rules == null) return;

        // 60초 경과 또는 외부에서 수동 정지 시 스폰 중단
        if (!stopAll && Time.timeSinceLevelLoad >= stopAfterSeconds) stopAll = true;
        if (stopAll) return;

        // GameManager를 이미 쓰고 있다면, 보스 타임에 맞춰 자동 중단도 지원
        if (GameManagerExistsAndBossTime()) { stopAll = true; return; }

        foreach (var r in rules)
        {
            if (r == null || r.prefab == null || r.maxConcurrent <= 0) continue;

            r.timer += Time.deltaTime;
            if (r.timer < r.interval) continue;

            // 타입별 동시 수 제한 체크
            if (!UnderLimit(r.typeId, r.maxConcurrent)) continue;

            r.timer = 0f;

            Vector3 pos = GetSmartSpawnPosition();
            var go = Instantiate(r.prefab, pos, Quaternion.identity);

            // 스폰 등록 + 파괴시 콜백 연결
            Register(r.typeId);
            AttachToken(go, r.typeId);
        }
    }

    public void StopAllSpawning()
    {
        stopAll = true;
    }

    bool GameManagerExistsAndBossTime()
    {
        // GameManager.Instance.IsBossTime == true 면 스폰 중단
        // GameManager가 없는 프로젝트여도 문제 없도록 null 안전 처리
        return (GameManager.Instance != null && GameManager.Instance.IsBossTime);
    }

    // ----- 동시 수 관리 -----
    bool UnderLimit(string typeId, int maxConcurrent)
    {
        int cur = GetCount(typeId);
        return cur < maxConcurrent;
    }

    void Register(string typeId)
    {
        if (string.IsNullOrEmpty(typeId)) return;
        if (!liveCounts.ContainsKey(typeId)) liveCounts[typeId] = 0;
        liveCounts[typeId]++;
    }

    void Unregister(string typeId)
    {
        if (string.IsNullOrEmpty(typeId)) return;
        if (!liveCounts.ContainsKey(typeId)) return;
        liveCounts[typeId] = Mathf.Max(0, liveCounts[typeId] - 1);
    }

    int GetCount(string typeId)
    {
        if (string.IsNullOrEmpty(typeId)) return 0;
        return liveCounts.TryGetValue(typeId, out var v) ? v : 0;
    }

    void AttachToken(GameObject go, string typeId)
    {
        if (go == null) return;
        var token = go.GetComponent<SpawnedEnemyToken>();
        if (token == null) token = go.AddComponent<SpawnedEnemyToken>();
        token.Bind(this, typeId);
    }

    // ----- 스폰 위치 계산 -----
    Vector3 GetSmartSpawnPosition()
    {
        for (int i = 0; i < Mathf.Max(1, maxSpawnTries); i++)
        {
            Vector3 pos = (spawnSpace == SpawnSpace.OutsideCamera)
                ? GetSpawnOutsideCamera(cameraMargin)
                : GetSpawnOnRingAroundPlayer(ringRadius);

            if (center == null || Vector2.Distance(pos, center.position) >= minDistanceFromPlayer)
                return pos;
        }
        return GetSpawnOnRingAroundPlayer(Mathf.Max(ringRadius, minDistanceFromPlayer + 1f));
    }

    Vector3 GetSpawnOutsideCamera(float margin)
    {
        var cam = Camera.main;
        if (cam == null) return Vector3.zero;

        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;
        Vector3 c = cam.transform.position;

        if (Random.value < 0.5f)
        {
            float y = (Random.value < 0.5f) ? (c.y + halfH + margin) : (c.y - halfH - margin);
            float x = Random.Range(c.x - halfW, c.x + halfW);
            return new Vector3(x, y, 0f);
        }
        else
        {
            float x = (Random.value < 0.5f) ? (c.x - halfW - margin) : (c.x + halfW + margin);
            float y = Random.Range(c.y - halfH, c.y + halfH);
            return new Vector3(x, y, 0f);
        }
    }

    Vector3 GetSpawnOnRingAroundPlayer(float radius)
    {
        Vector3 c = (center != null) ? center.position : Vector3.zero;
        float ang = Random.Range(0f, Mathf.PI * 2f);
        Vector3 offset = new Vector3(Mathf.Cos(ang), Mathf.Sin(ang), 0f) * Mathf.Max(0.01f, radius);
        return c + offset;
    }

    // 스폰된 적에 달려서 파괴될 때 카운트 차감하는 토큰
    private sealed class SpawnedEnemyToken : MonoBehaviour
    {
        EnemySpawnerLimited spawner;
        string typeId;

        public void Bind(EnemySpawnerLimited s, string t)
        {
            spawner = s; typeId = t;
        }

        void OnDestroy()
        {
            if (spawner != null) spawner.Unregister(typeId);
        }
    }
}
