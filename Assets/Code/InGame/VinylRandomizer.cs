using UnityEngine;
using UnityEngine.UI;

public class VinylRandomizer : MonoBehaviour
{
    private RectTransform rect;
    private Image img;
    private Canvas canvas;

    [Header("Size Range")]
    [SerializeField] private Vector2 sizeRange = new Vector2(500f, 900f); // 최소/최대 크기(px)

    [Header("Spawn Margin")]
    [SerializeField] private float margin = 100f; // 화면 가장자리 여백(px)

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        img = GetComponent<Image>();
        canvas = GetComponentInParent<Canvas>();
    }

    private void Start()
    {
        MoveToRandomPosition();
    }

    /// <summary>
    /// 캔버스 안 무작위 위치/크기로 이동
    /// </summary>
    public void MoveToRandomPosition()
    {
        if (rect == null || canvas == null) return;

        // 랜덤 크기 설정
        float size = Random.Range(sizeRange.x, sizeRange.y);
        rect.sizeDelta = new Vector2(size, size);

        // 화면 내부 좌표 계산
        var canvasRect = canvas.GetComponent<RectTransform>().rect;

        float x = Random.Range(canvasRect.xMin + margin, canvasRect.xMax - margin);
        float y = Random.Range(canvasRect.yMin + margin, canvasRect.yMax - margin);

        rect.anchoredPosition = new Vector2(x, y);

        // 약간의 회전 효과 추가 (선택)
        rect.localRotation = Quaternion.Euler(0, 0, Random.Range(-20f, 20f));

        // 약간의 반투명 (선택)
        if (img != null)
        {
            var c = img.color;
            c.a = Random.Range(0.7f, 0.9f);
            img.color = c;
        }
    }
}
