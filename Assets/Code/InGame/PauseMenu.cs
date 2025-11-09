using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Root")]
    public GameObject settingsCanvas;   // 설정 전체 캔버스 (비활성 시작)

    bool paused;

    void Start()
    {
        if (settingsCanvas) settingsCanvas.SetActive(false);
        Resume(); // 안전하게 초기화
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    public void TogglePause()
    {
        if (paused) Resume();
        else Pause();
    }

    public void Pause()
    {
        if (paused) return;
        paused = true;
        Time.timeScale = 0f;
        if (settingsCanvas) settingsCanvas.SetActive(true);
        // 커서 보이게
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        paused = false;
        Time.timeScale = 1f;
        if (settingsCanvas) settingsCanvas.SetActive(false);
        // 커서 정책은 게임에 맞게
        Cursor.visible = true;
    }

    // 옵션: 재시작 버튼에 연결
    public void Restart()
    {
        Time.timeScale = 1f;
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }
}
