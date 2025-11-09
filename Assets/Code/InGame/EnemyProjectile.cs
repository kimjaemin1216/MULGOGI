using UnityEngine;

public enum ProjectileKind { Damage, BlindStack, Homing, Split, Invisible }

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyProjectile : MonoBehaviour
{
    public ProjectileKind kind = ProjectileKind.Damage;

    [Header("Move")]
    public float speed = 3f;
    public bool homing = false;
    public Transform target;

    [Header("Life")]
    public float lifetime = 5f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    void Awake()
    {
        TryGetComponent(out rb);
        sr = GetComponent<SpriteRenderer>();
        if (rb != null)
        {
            rb.gravityScale = 0;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
    }

    void Start()
    {
        if (lifetime > 0f) Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (homing && target != null && rb != null)
        {
            Vector2 dir = ((Vector2)target.position - rb.position).normalized;
            rb.linearVelocity = dir * speed;
            transform.right = dir;
        }
    }

    public void Launch(Vector2 dir)
    {
        if (rb == null) return;
        rb.linearVelocity = dir.normalized * speed;
        transform.right = dir;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var hp = other.GetComponent<PlayerHealth>();
        if (hp == null) return;

        switch (kind)
        {
            case ProjectileKind.BlindStack:
                other.GetComponent<StatusController>()?.AddBlindStacks(1);
                break;

            default:
                hp.Damage(1);
                break;
        }

        Destroy(gameObject);
    }
}
