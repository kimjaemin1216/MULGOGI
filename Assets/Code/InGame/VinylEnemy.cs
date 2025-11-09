using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class VinylEnemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHP = 2;
    [SerializeField] private float speed = 1.8f;

    [Header("Sprites")]
    [SerializeField] private Sprite fullHP_Sprite; // A
    [SerializeField] private Sprite halfHP_Sprite; // H

    [Header("Center / Player")]
    [SerializeField] private Transform center;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private float facingOffsetDeg = 180f;

    [Header("Blind Settings")]
    [SerializeField] private float blindTouchCooldown = 0.8f;
    private float blindTimer = 0f;

    [Header("Screen Cover")]
    [SerializeField] private Sprite coverSprite;
    [SerializeField] private float coverHold = 0.7f;

    private int currentHP;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    // ✂️ Start()는 제거합니다 — 스폰 위치는 스포너가 정함

    private void Awake()
    {
        TryGetComponent(out rb);
        TryGetComponent(out sr);

        if (rb != null)
        {
            rb.gravityScale = 0;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        currentHP = maxHP;

        if (center == null)
        {
#if UNITY_2023_1_OR_NEWER
            var p = Object.FindFirstObjectByType<PlayerController>();
#else
            var p = Object.FindObjectOfType<PlayerController>();
#endif
            if (p != null) center = p.transform;
        }

        if (fullHP_Sprite != null && sr != null)
            sr.sprite = fullHP_Sprite;
    }

    private void Update()
    {
        if (center == null || rb == null) return;

        if (blindTimer > 0f) blindTimer -= Time.deltaTime;

        Vector2 dir = ((Vector2)center.position - rb.position).normalized;
        rb.linearVelocity = dir * speed;

        Vector2 facing = Quaternion.Euler(0, 0, facingOffsetDeg) * dir;
        transform.right = facing;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<BubbleProjectile>() != null)
        {
            TakeDamage(1);
            return;
        }

        if (other.TryGetComponent<PlayerHealth>(out var hp))
        {
            if (blindTimer <= 0f)
            {
                hp.AddHit(1);

                if (ScreenCoverMulti.Instance != null && coverSprite != null)
                    ScreenCoverMulti.Instance.Show(coverSprite, coverHold, 1);

                blindTimer = blindTouchCooldown;
                Die(); // 닿으면 비닐은 사라짐
            }
        }
    }

    private void TakeDamage(int dmg)
    {
        currentHP -= Mathf.Max(1, dmg);

        if (sr != null && halfHP_Sprite != null && currentHP <= maxHP / 2)
            sr.sprite = halfHP_Sprite;

        if (currentHP <= 0) Die();
    }

    private void Die()
    {
        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
