// BossController.cs (핵심 필드 + 패턴 부분만)
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BossController : MonoBehaviour
{
    [Header("Refs")]
    public Transform target;          // BossDirector가 넣어줌
    public GameObject vinylPrefab;    // "기존 비닐 프리팹" 1개

    [Header("SFX")]
    public AudioClip shootSfx;

    [Header("Pattern Timings")]
    public float patternGap = 1.2f;
    public float simpleInterval = 0.25f; // 일반 패턴 연사 간격
    public float homingBurstDelay = 0.8f;

    [Header("Pattern Params")]
    public int homingCount = 6;       // 유도탄 묶음 개수
    public float homingSpeed = 5f;
    public float homingTurnRate = 240f;

    public int ringBullets = 16;
    public float ringSpeed = 5f;

    public float simpleSpeed = 7f;
    public float simpleDuration = 2.5f;

    bool running;

    public void BeginPatterns()
    {
        if (running) return;
        running = true;
        StartCoroutine(PatternLoop());
    }

    IEnumerator PatternLoop()
    {
        while (true)
        {
            yield return StartCoroutine(Pattern_HomingBurst());
            yield return new WaitForSeconds(patternGap);

            yield return StartCoroutine(Pattern_Ring());
            yield return new WaitForSeconds(patternGap);

            yield return StartCoroutine(Pattern_SimpleStream());
            yield return new WaitForSeconds(patternGap);
        }
    }

    IEnumerator Pattern_HomingBurst()
    {
        if (!vinylPrefab || !target) yield break;

        for (int i = 0; i < homingCount; i++)
        {
            Vector3 off = new Vector3(0f, (i - homingCount * 0.5f) * 0.4f, 0f);
            var go = Instantiate(vinylPrefab, transform.position + off, Quaternion.identity);
            PrepareAsProjectile(go); // (아래 함수) 적 AI 제거/트리거 세팅

            var vp = go.AddComponent<VinylProjectile>();
            vp.Init(Vector2.left, homingSpeed, target, VinylProjectile.MoveType.Homing);
            vp.turnRate = homingTurnRate;

            PlayShotSfx();
        }
        yield return new WaitForSeconds(homingBurstDelay);
    }

    IEnumerator Pattern_Ring()
    {
        if (!vinylPrefab) yield break;

        for (int i = 0; i < ringBullets; i++)
        {
            float ang = (Mathf.PI * 2f) * i / ringBullets;
            Vector2 dir = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang));
            var go = Instantiate(vinylPrefab, transform.position, Quaternion.identity);
            PrepareAsProjectile(go);

            var vp = go.AddComponent<VinylProjectile>();
            vp.Init(dir, ringSpeed, null, VinylProjectile.MoveType.Straight);
        }
        PlayShotSfx();
        yield return null;
    }

    IEnumerator Pattern_SimpleStream()
    {
        if (!vinylPrefab) yield break;

        float t = 0f;
        while (t < simpleDuration)
        {
            t += simpleInterval;

            Vector2 dir = target ? ((Vector2)(target.position - transform.position)).normalized : Vector2.left;
            var go = Instantiate(vinylPrefab, transform.position, Quaternion.identity);
            PrepareAsProjectile(go);

            var vp = go.AddComponent<VinylProjectile>();
            vp.Init(dir, simpleSpeed, null, VinylProjectile.MoveType.Straight);

            PlayShotSfx();
            yield return new WaitForSeconds(simpleInterval);
        }
    }

    void PlayShotSfx()
    {
        if (shootSfx) AudioSource.PlayClipAtPoint(shootSfx, transform.position, 0.6f);
    }

    // 탄막으로 쓸 때 불필요한 컴포넌트를 걷어내고, 트리거/중력/질량을 안전하게 세팅
    void PrepareAsProjectile(GameObject go)
    {
        // (있으면) 일반 적 AI 제거
        var ai = go.GetComponent<MonoBehaviour>(); // 예: EnemyAI, Wander 등
        // 필요한 경우 구체 컴포넌트 이름으로 꺼내서 Destroy 해줘도 됨.
        // if (ai) Destroy(ai);

        var rb = go.GetComponent<Rigidbody2D>();
        if (rb)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
        var col = go.GetComponent<Collider2D>();
        if (col) col.isTrigger = true; // OnTriggerEnter2D로 Player에 데미지
    }
}
