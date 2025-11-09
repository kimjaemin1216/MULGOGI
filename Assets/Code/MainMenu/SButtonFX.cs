using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public void LoadInGame()
    {
        SceneManager.LoadScene("InGame"); // 씬 이름 정확히
    }
}
