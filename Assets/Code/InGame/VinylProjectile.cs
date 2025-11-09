// VinylProjectile.cs  (공용)
using UnityEngine;

public class VinylProjectile : MonoBehaviour
{
    public enum MoveType { Straight, Homing }

    public MoveType moveType = MoveType.Straight;
    public float speed = 5f;
    public float turnRate = 240f; // deg/sec
    public float life = 8f;
    public int damageHearts = 1;
    public Transform target;

    Vector2 dir = Vector2.left;

    public void Init(Vector2 d, float s, Transform t = null, MoveType type = MoveType.Straight)
    {
        dir = d.normalized;
        speed = s;
        moveType = type;
        target = t;
        float z = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, z);
    }

    void Start(){ Destroy(gameObject, life); }

    void Update()
    {
        if (moveType == MoveType.Homing && target)
        {
            Vector2 desired = ((Vector2)(target.position - transform.position)).normalized;
            float maxStep = turnRate * Mathf.Deg2Rad * Time.deltaTime;
            float ang = Vector2.SignedAngle(dir, desired) * Mathf.Deg2Rad;
            float step = Mathf.Clamp(ang, -maxStep, maxStep);
            float ca = Mathf.Cos(step), sa = Mathf.Sin(step);
            dir = new Vector2(dir.x * ca - dir.y * sa, dir.x * sa + dir.y * ca).normalized;
        }

        transform.position += (Vector3)(dir * speed * Time.deltaTime);
        float z2 = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, z2);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var hp = other.GetComponent<PlayerHealth>();
        if (hp)
        {
            hp.Damage(damageHearts);
            Destroy(gameObject);
        }
    }
}
