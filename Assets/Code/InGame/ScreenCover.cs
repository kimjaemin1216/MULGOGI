using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenCover : MonoBehaviour
{
    public static ScreenCover Instance;

    [Header("Refs")]
    [SerializeField] private Image coverImage;          // Canvas 안의 Image
    private RectTransform rect;                          // coverImage의 RectTransform
    private RectTransform canvasRect;                    // 부모 Canvas의 RectTransform

    [Header("Random Placement")]
    [SerializeField] private Vector2 sizeRange = new Vector2(600f, 1100f); // px
    [SerializeField] private float margin = 120f;        // 화면 가장자리 여백(px)
    [SerializeField] private float maxRotation = 20f;    // 무작위 회전 각도

    [Header("Fade")]
    [SerializeField] private float fadeIn  = 0.12f;
    [SerializeField] private float hold    = 0.70f;
    [SerializeField] private float fadeOut = 0.35f;

    private Coroutine routine;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (coverImage == null) coverImage = GetComponent<Image>();
        rect = coverImage != null ? coverImage.rectTransform : null;

        var canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas != null ? canvas.GetComponent<RectTransform>() : null;

        if (coverImage != null)
        {
            // 기본 세팅: 완전 투명 + 입력 차단 X + 비활성
            var c = coverImage.color; c.a = 0f; coverImage.color = c;
            coverImage.raycastTarget = false;
            coverImage.preserveAspect = true;
            coverImage.enabled = false;
        }
    }

    /// <summary> 스프라이트를 무작위 위치/크기로 보여줌 </summary>
    public void Show(Sprite sprite, float? holdOverride = null)
    {
        if (coverImage == null || rect == null || canvasRect == null || sprite == null) return;

        coverImage.sprite = sprite;
        if (holdOverride.HasValue) hold = Mathf.Max(0f, holdOverride.Value);

        // ★ 보이기 전에 무조건 무작위 배치
        RandomizePlacement();

        if (routine != null) StopCoroutine(routine);
        coverImage.enabled = true;
        routine = StartCoroutine(FadeRoutine());
    }

    private void RandomizePlacement()
    {
        // 크기
        float size = Random.Range(sizeRange.x, sizeRange.y);
        rect.sizeDelta = new Vector2(size, size);

        // 부모(Canvas) 영역 안에서 무작위 위치
        var area = canvasRect.rect;
        float x = Random.Range(area.xMin + margin, area.xMax - margin);
        float y = Random.Range(area.yMin + margin, area.yMax - margin);
        rect.anchoredPosition = new Vector2(x, y);

        // 약간의 회전
        rect.localRotation = Quaternion.Euler(0, 0, Random.Range(-maxRotation, maxRotation));
    }

    private IEnumerator FadeRoutine()
    {
        float t = 0f;
        var c = coverImage.color;

        // In
        while (t < fadeIn)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, fadeIn <= 0f ? 1f : t / fadeIn);
            coverImage.color = c;
            yield return null;
        }

        // Hold
        yield return new WaitForSeconds(hold);

        // Out
        t = 0f;
        while (t < fadeOut)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, fadeOut <= 0f ? 1f : t / fadeOut);
            coverImage.color = c;
            yield return null;
        }

        c.a = 0f; coverImage.color = c;
        coverImage.enabled = false;
        routine = null;
    }
}
