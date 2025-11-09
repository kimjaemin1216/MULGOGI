using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AllButton : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
{
    [Header("Refs")]
    public Image background;   // 버튼 배경 (Button에 붙은 Image)
    public Text label;         // 버튼 안의 텍스트
    public GameObject arrow;   // 왼쪽 화살표 Text 오브젝트

    [Header("Colors")]
    public Color normalBG = new Color(1f, 1f, 1f, 0.8f);   // 반투명 흰색
    public Color hoverBG  = new Color(0f, 0.75f, 1f, 0.85f); // 파랑
    public Color pressedBG= new Color(0f, 0.4f, 0.7f, 0.95f);

    public Color normalText = new Color(0f, 0.2f, 0.4f, 1f); // 네이비
    public Color hoverText  = Color.white;
    public Color pressedText= Color.white;

    [Header("Scale")]
    public float hoverScale   = 1.1f;
    public float pressedScale = 0.9f;
    public float speed        = 10f;

    bool hovered, pressed;
    Vector3 baseScale;

    void Awake()
    {
        if (background == null) background = GetComponent<Image>();
        if (label == null) label = GetComponentInChildren<Text>();
        baseScale = transform.localScale;

        if (arrow != null) arrow.SetActive(false);

        // 초기 색
        background.color = normalBG;
        label.color = normalText;
    }

    void Update()
    {
        float targetScale = 1f;
        Color bg = normalBG;
        Color txt = normalText;

        if (pressed)
        {
            targetScale = pressedScale;
            bg = pressedBG;
            txt = pressedText;
        }
        else if (hovered)
        {
            targetScale = hoverScale;
            bg = hoverBG;
            txt = hoverText;
        }

        transform.localScale = Vector3.Lerp(transform.localScale, baseScale * targetScale, Time.deltaTime * speed);
        background.color = Color.Lerp(background.color, bg, Time.deltaTime * speed);
        label.color = Color.Lerp(label.color, txt, Time.deltaTime * speed);
    }

    public void OnPointerEnter(PointerEventData e) { hovered = true; if (arrow) arrow.SetActive(true); }
    public void OnPointerExit (PointerEventData e) { hovered = false; if (arrow) arrow.SetActive(false); }
    public void OnPointerDown (PointerEventData e) { pressed = true; }
    public void OnPointerUp   (PointerEventData e) { pressed = false; }
}
