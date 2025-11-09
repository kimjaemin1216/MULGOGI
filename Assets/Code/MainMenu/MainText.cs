using UnityEngine;

public class MainText : MonoBehaviour
{
    public float scaleAmp = 0.06f;   // 크기 변화 폭
    public float rotAmp   = 2.0f;    // 회전 변화(도)
    public float speed    = 1.2f;    // 속도

    Vector3 baseScale;
    float baseRotZ;

    void Awake()
    {
        baseScale = transform.localScale;
        baseRotZ  = transform.eulerAngles.z;
    }

    void Update()
    {
        float s = 1f + Mathf.Sin(Time.time * speed) * scaleAmp;
        transform.localScale = baseScale * s;

        float r = baseRotZ + Mathf.Sin(Time.time * (speed * 0.6f)) * rotAmp;
        transform.rotation = Quaternion.Euler(0, 0, r);
    }
}
