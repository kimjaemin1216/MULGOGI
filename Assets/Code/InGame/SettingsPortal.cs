using UnityEngine;
using UnityEngine.SceneManagement;

public static class SettingsPortal
{
    static bool _frozeTime;
    static int _returnSceneBuildIndex = -1;

    /// <summary>
    /// Settings 씬을 Additive로 연다.
    /// </summary>
    public static void Open(bool freezeTime)
    {
        var active = SceneManager.GetActiveScene();
        _returnSceneBuildIndex = active.buildIndex;
        _frozeTime = freezeTime;

        if (_frozeTime) Time.timeScale = 0f; // 인게임에서 열면 정지

        // 이미 열려 있으면 중복 방지
        if (!SceneManager.GetSceneByName("Settings").isLoaded)
        {
            SceneManager.LoadSceneAsync("Settings", LoadSceneMode.Additive);
        }
    }

    /// <summary>
    /// Settings 씬을 닫는다.
    /// </summary>
    public static void Close()
    {
        if (SceneManager.GetSceneByName("Settings").isLoaded)
        {
            SceneManager.UnloadSceneAsync("Settings");
        }

        if (_frozeTime) Time.timeScale = 1f; // 정지 복구
        _frozeTime = false;
        _returnSceneBuildIndex = -1;
    }
}
