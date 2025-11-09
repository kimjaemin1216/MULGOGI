using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenCoverMulti : MonoBehaviour
{
    public static ScreenCoverMulti Instance;

    [Header("Parent Layer")]
    [SerializeField] private RectTransform layer; // Canvas 밑에 전체 화면 RectTransform

    [Header("Random Placement")]
    [SerializeField] private Vector2 sizeRange = new Vector2(600f, 1100f); // px
    [SerializeField] private float margin = 120f;     // 화면 가장자리 여백(px)
    [SerializeField] private float maxRotation = 20f; // 무작위 회전 각도

    [Header("Fade (seconds)")]
    [SerializeField] private float fadeIn  = 0.12f;
    [SerializeField] private float hold    = 0.70f;
    [SerializeField] private float fadeOut = 0.35f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (layer == null) layer = GetComponent<RectTransform>();
        if (layer == null)
        {
            var cv = GetComponentInParent<Canvas>();
            if (cv != null) layer = cv.GetComponent<RectTransform>();
        }
    }

    /// <summary>
    /// 스프라이트를 n장 생성해 무작위 위치/크기/회전으로 나타나게 함
    /// </summary>
    public void Show(Sprite sprite, float? holdOverride = null, int count = 1)
    {
        if (sprite == null || layer == null || count <= 0) return;

        float h = holdOverride.HasValue ? Mathf.Max(0f, holdOverride.Value) : hold;

        for (int i = 0; i < count; i++)
            StartCoroutine(SpawnOne(sprite, h));
    }

    private IEnumerator SpawnOne(Sprite sprite, float holdTime)
    {
        // UI Image 동적 생성
        var go  = new GameObject("CoverDecal", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        var rt  = go.GetComponent<RectTransform>();
        var img = go.GetComponent<Image>();

        go.transform.SetParent(layer, false);
        img.sprite = sprite;
        img.preserveAspect = true;
        img.raycastTarget = false; // 입력 방해 X
        img.color = new Color(1f, 1f, 1f, 0f); // 처음엔 투명

        // 무작위 크기/위치/회전
        float size = Random.Range(sizeRange.x, sizeRange.y);
        rt.sizeDelta = new Vector2(size, size);

        var area = layer.rect;
        float x = Random.Range(area.xMin + margin, area.xMax - margin);
        float y = Random.Range(area.yMin + margin, area.yMax - margin);
        rt.anchoredPosition = new Vector2(x, y);

        float rotZ = Random.Range(-maxRotation, maxRotation);
        rt.localRotation = Quaternion.Euler(0, 0, rotZ);

        // 페이드 인
        float t = 0f; var c = img.color;
        while (t < fadeIn)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, fadeIn <= 0f ? 1f : t / fadeIn);
            img.color = c;
            yield return null;
        }

        // 유지
        yield return new WaitForSeconds(holdTime);

        // 페이드 아웃
        t = 0f;
        while (t < fadeOut)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, fadeOut <= 0f ? 1f : t / fadeOut);
            img.color = c;
            yield return null;
        }

        Destroy(go);
    }
}
