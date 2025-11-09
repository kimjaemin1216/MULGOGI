using UnityEngine;
public class ScreenShake2D : MonoBehaviour
{
    Vector3 origin;
    void Awake(){ origin = transform.localPosition; }
    public void Shake(float duration = 0.4f, float strength = 0.3f)
    {
        StopAllCoroutines();
        StartCoroutine(DoShake(duration, strength));
    }
    System.Collections.IEnumerator DoShake(float d, float s)
    {
        float t = 0f;
        while (t < d)
        {
            t += Time.unscaledDeltaTime;
            transform.localPosition = origin + (Vector3)Random.insideUnitCircle * s;
            yield return null;
        }
        transform.localPosition = origin;
    }
}
