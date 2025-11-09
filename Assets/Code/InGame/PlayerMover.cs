// PlayerMover.cs  (플레이어에 붙이기)
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMover : MonoBehaviour
{
    public float moveSpeed = 6f;
    Rigidbody2D rb;

    void Awake(){ rb = GetComponent<Rigidbody2D>(); rb.gravityScale = 0f; }

    void Update()
    {
        if (GameOverController.Instance && Time.timeScale == 0f) { rb.linearVelocity = Vector2.zero; return; }

        float x = Input.GetAxisRaw("Horizontal"); // A/D 또는 ←/→
        float y = Input.GetAxisRaw("Vertical");   // W/S 또는 ↑/↓
        Vector2 v = new Vector2(x, y).normalized * moveSpeed;
        rb.linearVelocity = v;
    }

    // 보스 등장 시 살짝 왼쪽으로 밀고 회전 원복
    public void NudgeLeftAndReset(float dx, float time)
    {
        StopAllCoroutines();
        StartCoroutine(DoNudge(dx, time));
    }

    System.Collections.IEnumerator DoNudge(float dx, float time)
    {
        Vector3 s = transform.position, e = s + new Vector3(dx, 0, 0);
        Quaternion r0 = transform.rotation;
        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            float k = Mathf.SmoothStep(0, 1, t / time);
            transform.position = Vector3.Lerp(s, e, k);
            transform.rotation = Quaternion.Slerp(r0, Quaternion.identity, k);
            yield return null;
        }
        transform.position = e;
        transform.rotation = Quaternion.identity;
    }
}
