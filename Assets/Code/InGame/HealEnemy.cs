// Assets/Code/InGame/HealEnemy.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class HealEnemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHP = 3;     // 버블로 3번 맞아야 잡히는 등
    [SerializeField] private float speed = 1.2f;
    [SerializeField] private int healAmount = 1;

    [Header("Center/Target")]
    [SerializeField] private Transform center;  // 보통 Player(중앙)
    [SerializeField] private PlayerHealth player; // 회복 줄 대상

    [Header("FX (optional)")]
    [SerializeField] private GameObject deathEffect; // 잡혔을 때 파티클 등

    private int currentHP;
    private Rigidbody2D rb;
    private Collider2D col;

    private void Awake()
    {
        TryGetComponent(out rb);
        TryGetComponent(out col);

        if (rb != null)
        {
            rb.gravityScale = 0;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        currentHP = maxHP;

        // 레퍼런스 자동 배치 (직접 드래그하면 이 과정 생략 가능)
        if (center == null)
        {
        #if UNITY_2023_1_OR_NEWER
            var p = Object.FindFirstObjectByType<PlayerController>();
        #else
            var p = Object.FindObjectOfType<PlayerController>();
        #endif
            if (p != null) center = p.transform;
        }

        if (player == null)
        {
        #if UNITY_2023_1_OR_NEWER
            player = Object.FindFirstObjectByType<PlayerHealth>();
        #else
            player = Object.FindObjectOfType<PlayerHealth>();
        #endif
        }

        // 플레이어와 부딪혀도 피해 주지 않으므로 트리거 권장
        if (col != null) col.isTrigger = true;
    }

    private void Update()
    {
        if (rb == null || center == null) return;

        Vector2 dir = ((Vector2)center.position - rb.position).normalized;
        rb.linearVelocity = dir * speed;   // 프로젝트에서 linearVelocity 사용 중
        transform.right = dir;             // 바라보는 방향 정렬
    }

    // 버블에 맞으면 체력 감소 → 0이면 플레이어 회복 + 사라짐
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어의 버블에만 반응
        if (other.GetComponent<BubbleProjectile>() == null) return;

        TakeDamage(1);
    }

    private void TakeDamage(int dmg)
    {
        currentHP -= Mathf.Max(1, dmg);
        if (currentHP <= 0)
        {
            if (player != null) player.Heal(healAmount);

            if (deathEffect != null)
                Instantiate(deathEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
