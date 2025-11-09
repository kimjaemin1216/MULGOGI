using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartBar : MonoBehaviour
{
    public static HeartBar Instance;

    [Header("Prefabs & Options")]
    [SerializeField] private GameObject heartPrefab;     // UI > Image로 만든 하트 프리팹
    [SerializeField, Range(0,5)] private int maxHearts = 5;
    [SerializeField] private Vector2 heartSize = new Vector2(48, 48); // ★ 사이즈 강제
    [SerializeField] private Sprite heartSprite;         // ★ 비어있으면 프리팹 그대로 사용

    private readonly List<GameObject> hearts = new();
    private PlayerHealth player;

    void Awake()
    {
        if (Instance == null) Instance = this;

        // 우상단 고정(안전망)
        var rt = (RectTransform)transform;
        rt.anchorMin = rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(1f, 1f);
        if (rt.anchoredPosition == Vector2.zero) rt.anchoredPosition = new Vector2(-20f, -20f);

        Build(maxHearts);
        SetHearts(maxHearts); // 시작: 풀피 가정
    }

    void Start()
    {
        // 플레이어 자동 연결 → 체력 이벤트로 동기화
        player = FindAnyObjectByType<PlayerHealth>();
        if (player != null)
        {
            SetMax(player.MaxHearts);
            SetHearts(player.CurrentHearts);
            player.OnHealthChanged += OnHealthChanged;
        }
    }

    void OnDestroy()
    {
        if (player != null) player.OnHealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(int current, int max)
    {
        SetMax(max);
        SetHearts(current);
    }

    public void Build(int max)
    {
        max = Mathf.Clamp(max, 0, 5);

        // 기존 제거
        foreach (Transform t in transform) Destroy(t.gameObject);
        hearts.Clear();

        for (int i = 0; i < max; i++)
        {
            var go = Instantiate(heartPrefab, transform);

            // ★ 반드시 사이즈/스프라이트 보정
            var img = go.GetComponent<Image>();
            if (img != null)
            {
                if (heartSprite != null) img.sprite = heartSprite;
                img.color = Color.white;
                img.raycastTarget = false;
                img.preserveAspect = true;
            }
            var hrt = go.GetComponent<RectTransform>();
            if (hrt != null)
            {
                hrt.anchorMin = hrt.anchorMax = new Vector2(0f, 1f); // 왼쪽-위 기준으로
                hrt.pivot = new Vector2(0f, 1f);
                hrt.sizeDelta = heartSize;                           // ★ 48x48 등
                hrt.anchoredPosition = Vector2.zero;                 // 레이아웃이 배치
                hrt.localScale = Vector3.one;
            }

            hearts.Add(go);
        }

        // ★ 레이아웃 안전망: 없으면 자동 추가
        if (GetComponent<HorizontalLayoutGroup>() == null)
        {
            var layout = gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 8f;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = layout.childControlHeight = false;
            layout.childForceExpandWidth = layout.childForceExpandHeight = false;
        }
        if (GetComponent<ContentSizeFitter>() == null)
        {
            var fitter = gameObject.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }

    public void SetHearts(int current)
    {
        current = Mathf.Clamp(current, 0, hearts.Count);
        for (int i = 0; i < hearts.Count; i++)
            hearts[i].SetActive(i < current);
    }

    public void SetMax(int newMax)
    {
        newMax = Mathf.Clamp(newMax, 0, 5);
        if (newMax != hearts.Count) Build(newMax);
    }
}
