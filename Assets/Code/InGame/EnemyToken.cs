using UnityEngine;

public class EnemyToken : MonoBehaviour
{
    [Tooltip("이 타입의 고유 ID. 예: Vinyl, Glass, Styro, Can, Plastic, TrashCanBoss")]
    public string typeId = "Enemy";

    private void OnEnable()  => EnemyLimiter.Register(typeId);
    private void OnDisable() => EnemyLimiter.Unregister(typeId);
    // 비활성/파괴 모두 커버 (파괴 시에도 OnDisable 호출됨)
}
