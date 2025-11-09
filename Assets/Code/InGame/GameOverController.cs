using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public static GameOverController Instance { get; private set; }
    [SerializeField] GameObject gameOverPanel;

    bool shown;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // 혹시 이전에 멈춰있을 수 있으니 안전하게 1로 복구
        Time.timeScale = 1f;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        shown = false;
    }

    public void ShowGameOver()
    {
        if (shown) return;
        shown = true;

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f; // 정지
    }

    // 버튼에 연결: "다시 시작"
    public void OnClickRestart()
    {
        Time.timeScale = 1f;

        // 현재 활성 씬을 그대로 다시 로드 (완전 초기화)
        var active = SceneManager.GetActiveScene();
        SceneManager.LoadScene(active.buildIndex);
    }
}
