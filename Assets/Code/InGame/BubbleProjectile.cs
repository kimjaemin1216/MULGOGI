using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BubbleProjectile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifetime = 3f;

    [Header("Pop Effect")]
    [SerializeField] private GameObject popEffectPrefab;
    [SerializeField] private bool spawnEffectOffscreen = false;

    [Header("Screen Edges")]
    [SerializeField, Range(0f, 0.15f)] private float edgePopInset = 0.01f;
    private Rigidbody2D rb;

    private void Awake()
    {
        TryGetComponent(out rb);
    }

    public void Launch(Vector2 direction, float customSpeed = -1f)
    {
        if (customSpeed > 0f) speed = customSpeed;
        transform.right = direction.normalized;
        if (rb != null) rb.linearVelocity = direction.normalized * speed;

        // 수명 타임아웃도 "보이는 위치"에서 터지도록 처리
        Invoke(nameof(TimeoutPop), lifetime);
    }

    private void Update()
    {
        if (Camera.main == null) return;

        // 현재 뷰포트 좌표(0~1)
        Vector3 vp = Camera.main.WorldToViewportPoint(transform.position);

        // 1) 화면 밖으로 완전히 나가기 전에 안쪽에서 터뜨리기
        bool nearEdge =
            (vp.x <= edgePopInset) || (vp.x >= 1f - edgePopInset) ||
            (vp.y <= edgePopInset) || (vp.y >= 1f - edgePopInset);

        if (nearEdge)
        {
            Vector3 clampedVp = new Vector3(
                Mathf.Clamp01(vp.x),
                Mathf.Clamp01(vp.y),
                vp.z
            );

            // 안쪽으로 살짝 끌어온(인셋 적용) 위치
            clampedVp.x = Mathf.Clamp(clampedVp.x, edgePopInset, 1f - edgePopInset);
            clampedVp.y = Mathf.Clamp(clampedVp.y, edgePopInset, 1f - edgePopInset);

            Vector3 worldPos = Camera.main.ViewportToWorldPoint(clampedVp);
            DestroySelfWithEffect(worldPos);
        }

        // 2) 만약 이미 화면 밖으로 나갔다면
        if (vp.x < 0f || vp.x > 1f || vp.y < 0f || vp.y > 1f)
        {
            if (spawnEffectOffscreen)
            {
                // 밖이라도 효과를 보여주고 싶다면: 가장 가까운 가장자리로 끌어와서 표시
                Vector3 clampedVp = new Vector3(Mathf.Clamp01(vp.x), Mathf.Clamp01(vp.y), vp.z);
                Vector3 worldPos = Camera.main.ViewportToWorldPoint(clampedVp);
                DestroySelfWithEffect(worldPos);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private void TimeoutPop()
    {
        if (Camera.main == null) { DestroySelfWithEffect(transform.position); return; }

        // 수명 종료 시에도 화면 안이라면 그대로, 가장자리에 가깝다면 인셋 내로
        Vector3 vp = Camera.main.WorldToViewportPoint(transform.position);
        Vector3 clampedVp = new Vector3(
            Mathf.Clamp(vp.x, edgePopInset, 1f - edgePopInset),
            Mathf.Clamp(vp.y, edgePopInset, 1f - edgePopInset),
            vp.z
        );
        Vector3 worldPos = Camera.main.ViewportToWorldPoint(clampedVp);
        DestroySelfWithEffect(worldPos);
    }

    // pos가 주어지면 그 위치에 이펙트를 생성
    private void DestroySelfWithEffect(Vector3? posOverride = null)
    {
        if (popEffectPrefab != null)
        {
            Vector3 spawnPos = posOverride ?? transform.position;
            Instantiate(popEffectPrefab, spawnPos, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DestroySelfWithEffect(transform.position);
    }
}
