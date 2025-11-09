using UnityEngine;

[RequireComponent(typeof(PlayerHealth))]
public class StatusController : MonoBehaviour
{
    [Header("Blind (Vinyl)")]
    [SerializeField] private int   stacksToDamage     = 3;   // 3스택 시 HP -1
    [SerializeField] private float stackDecayInterval = 3f;  // n초마다 스택 1 감소
    [SerializeField] private int   maxBlindStacks     = 3;   // 시야 이펙트 상한

    [Header("Optional refs")]
    [SerializeField] private ScreenVignette vignette; // 인스펙터로 연결(없으면 자동 탐색)

    public int BlindStacks { get; private set; } = 0;

    private float decayTimer = 0f;
    private PlayerHealth hp;

    private void Awake()
    {
        TryGetComponent(out hp);

        // 인스펙터에서 비어 있으면 한 번만 자동 탐색
        if (vignette == null)
            vignette = FindVignetteOnce();
    }

    private ScreenVignette FindVignetteOnce()
    {
        // Unity 2023.1 이상: 새 API 사용
        #if UNITY_2023_1_OR_NEWER
        return Object.FindFirstObjectByType<ScreenVignette>(); // 또는 FindAnyObjectByType<ScreenVignette>()
        #else
        return Object.FindObjectOfType<ScreenVignette>();
        #endif
    }

    private void Update()
    {
        if (BlindStacks <= 0) return;

        decayTimer += Time.deltaTime;
        if (decayTimer >= Mathf.Max(0.01f, stackDecayInterval))
        {
            decayTimer = 0f;
            AddBlindStacks(-1, applyDamageThreshold: false);
        }
    }

    /// <summary>블라인드 스택 증감.</summary>
    public void AddBlindStacks(int delta, bool applyDamageThreshold = true)
    {
        int before = BlindStacks;
        BlindStacks = Mathf.Clamp(BlindStacks + delta, 0, Mathf.Max(1, maxBlindStacks));

        if (vignette != null)
            vignette.SetBlindness(BlindStacks, maxBlindStacks);

        if (applyDamageThreshold && before < stacksToDamage && BlindStacks >= stacksToDamage)
        {
            if (hp != null) hp.Damage(1);
            ResetBlind();
        }
    }

    public void ResetBlind()
    {
        BlindStacks = 0;
        decayTimer = 0f;
        if (vignette != null)
            vignette.SetBlindness(0, maxBlindStacks);
    }
}
