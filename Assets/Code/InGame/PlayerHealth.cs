using System;
using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class PlayerHealth : MonoBehaviour
{
    [Header("Hearts")]
    [SerializeField] private int maxHearts = 3;   // 0~5 권장
    [SerializeField] private int startHearts = 3; // 0~max

    [Header("Hit → Heart rule")]
    [SerializeField] private int hitsPerHeart = 3;   // 3대 맞으면 1칸 감소
    private int pendingHits = 0;                     // 누적 피격 수

    [Header("Invincibility")]
    [SerializeField] private float invincibleDuration = 3f;
    [SerializeField] private float blinkInterval = 0.12f;

    [Header("On Death Actions")]
    [SerializeField] private bool disableColliderOnDeath = true;
    [SerializeField] private bool disableMovementOnDeath = true; // PlayerController 등 비활성화

    public int CurrentHearts { get; private set; }
    public int MaxHearts => maxHearts;
    public bool IsInvincible { get; private set; }
    public bool IsDead { get; private set; }

    // (현재, 최대)
    public event Action<int, int> OnHealthChanged;
    public event Action OnDied;

    private SpriteRenderer[] renderers;
    private Collider2D col;
    private Coroutine invCo;

    void Awake()
    {
        col = GetComponent<Collider2D>();

        // 시작 체력: 0~max 허용
        CurrentHearts = Mathf.Clamp(startHearts, 0, maxHearts);
        renderers = GetComponentsInChildren<SpriteRenderer>(true);

        // 초기 이벤트 발행 (Start에서 HeartBar 구독 후 한 번 더 싱크해줌)
        OnHealthChanged?.Invoke(CurrentHearts, maxHearts);
    }

    void Start()
    {
        // HeartBar 자동 싱크 (있을 때만)
        OnHealthChanged += SyncHeartBar;
        SyncHeartBar(CurrentHearts, maxHearts);
    }

    private void OnDestroy()
    {
        OnHealthChanged -= SyncHeartBar;
    }

    private void SyncHeartBar(int current, int max)
    {
        HeartBar.Instance?.SetMax(max);
        HeartBar.Instance?.SetHearts(current);
    }

    /// 외부에서 '한 대' 맞았다고 호출 (비닐 등 스택형 공격)
    public void AddHit(int hits = 1)
    {
        if (IsInvincible || IsDead) return;

        pendingHits += Mathf.Max(1, hits);

        while (pendingHits >= hitsPerHeart)
        {
            pendingHits -= hitsPerHeart;
            ApplyHeartDamage(1);
            if (IsInvincible || IsDead) break;
        }
    }

    /// 직접 하트 데미지(투사체 등 즉시 1칸 감소)
    public void Damage(int hearts = 1)
    {
        if (IsInvincible || IsDead) return;
        ApplyHeartDamage(Mathf.Max(1, hearts));
    }

    /// 하트 회복
    public void Heal(int hearts = 1)
    {
        if (IsDead) return;

        int prev = CurrentHearts;
        CurrentHearts = Mathf.Min(CurrentHearts + Mathf.Max(1, hearts), maxHearts);
        if (CurrentHearts != prev) OnHealthChanged?.Invoke(CurrentHearts, maxHearts);
    }

    /// 최대 하트 변경(업그레이드 등)
    public void SetMaxHearts(int newMax, bool keepCurrentRatio = true)
    {
        if (IsDead) return;

        newMax = Mathf.Clamp(newMax, 0, 5);
        if (newMax == maxHearts) return;

        if (keepCurrentRatio && maxHearts > 0)
        {
            float ratio = (float)CurrentHearts / maxHearts;
            maxHearts = newMax;
            CurrentHearts = Mathf.Clamp(Mathf.RoundToInt(ratio * maxHearts), 0, maxHearts);
        }
        else
        {
            maxHearts = newMax;
            CurrentHearts = Mathf.Clamp(CurrentHearts, 0, maxHearts);
        }

        OnHealthChanged?.Invoke(CurrentHearts, maxHearts);
    }

    // ===== 내부 처리 =====

    private void ApplyHeartDamage(int hearts)
    {
        if (IsInvincible || IsDead) return;

        CurrentHearts = Mathf.Max(0, CurrentHearts - Mathf.Max(1, hearts));
        OnHealthChanged?.Invoke(CurrentHearts, maxHearts);

        if (CurrentHearts <= 0 && !IsDead)
        {
            Die();
            return;
        }

        if (invCo != null) StopCoroutine(invCo);
        invCo = StartCoroutine(InvincibleRoutine());
    }

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;

        if (disableColliderOnDeath && col != null) col.enabled = false;
        if (disableMovementOnDeath)
        {
            // TODO: 실제 이동/공격 스크립트 비활성화 코드로 교체
            var pc = GetComponent<MonoBehaviour>();
            if (pc) pc.enabled = false;
        }

        if (invCo != null) { StopCoroutine(invCo); invCo = null; }
        SetAlpha(1f);
        IsInvincible = false;

        OnDied?.Invoke();

        if (GameOverController.Instance != null)
            GameOverController.Instance.ShowGameOver();
        else
            Debug.LogWarning("[PlayerHealth] GameOverController.Instance가 없어 게임오버 패널을 띄우지 못했습니다.");
    }

    private IEnumerator InvincibleRoutine()
    {
        IsInvincible = true;

        float t = 0f;
        bool visible = true;

        while (t < invincibleDuration)
        {
            t += blinkInterval;
            visible = !visible;
            SetAlpha(visible ? 1f : 0.25f);
            yield return new WaitForSeconds(blinkInterval);
        }

        SetAlpha(1f);
        IsInvincible = false;
        invCo = null;
    }

    private void SetAlpha(float a)
    {
        if (renderers == null) return;
        foreach (var r in renderers)
        {
            if (r == null) continue;
            var c = r.color; c.a = a; r.color = c;
        }
    }

    // === 저장값 복원용 ===
    public void SetCurrentHearts(int hearts, bool resetInvincibility = true)
    {
        IsDead = false;
        if (disableColliderOnDeath && col != null) col.enabled = true;

        CurrentHearts = Mathf.Clamp(hearts, 0, maxHearts);
        pendingHits = 0;

        if (resetInvincibility)
        {
            if (invCo != null) { StopCoroutine(invCo); invCo = null; }
            SetAlpha(1f);
            IsInvincible = false;
        }

        OnHealthChanged?.Invoke(CurrentHearts, maxHearts);
    }
}
