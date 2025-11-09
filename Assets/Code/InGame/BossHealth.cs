using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [SerializeField] int maxHP = 100;
    int hp;

    public System.Action OnBossDead;

    void Awake()
    {
        hp = maxHP;
    }

    public void TakeDamage(int dmg)
    {
        if (hp <= 0) return;
        hp = Mathf.Max(0, hp - Mathf.Max(1, dmg));
        if (hp == 0) Die();
    }

    void Die()
    {
        OnBossDead?.Invoke();
        Destroy(gameObject);
        // TODO: 클리어 UI, 다음 챕터, 드랍 등은 여기서 이어가면 됨
    }
}
