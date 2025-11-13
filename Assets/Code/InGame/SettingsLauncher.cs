using UnityEngine;

public class SettingsLauncher : MonoBehaviour
{
    // 메인메뉴: 시간 정지 없이 단순 오버레이
    public void OpenFromMenu()
    {
        SettingsPortal.Open(false);
    }

    // 인게임: 시간 정지하고 열기
    public void OpenFromInGame()
    {
        SettingsPortal.Open(true);
    }
}
