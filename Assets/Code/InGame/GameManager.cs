using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] public float bossTime = 60f;
    public bool IsBossTime { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Update()
    {
        if (!IsBossTime && Time.timeSinceLevelLoad >= bossTime)
        {
            IsBossTime = true;
        }
    }
}
