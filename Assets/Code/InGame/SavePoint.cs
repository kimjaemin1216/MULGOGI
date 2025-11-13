using UnityEngine;
using UnityEngine.SceneManagement;

public class SavePoint : MonoBehaviour
{
    [Tooltip("null이면 태그 'Player'로 자동 찾습니다.")]
    public Transform player;

    [ContextMenu("Save Now (Context Menu)")]
    public void SaveNow()
    {
        // 플레이어 참조 보정
        if (player == null)
        {
            var go = GameObject.FindWithTag("Player");
            if (go) player = go.transform;
        }
        if (player == null)
        {
            Debug.LogWarning("[SavePoint] Player가 없어 저장을 건너뜁니다.");
            return;
        }

        var ph = player.GetComponent<PlayerHealth>();
        var data = new SaveData
        {
            scene = SceneManager.GetActiveScene().name,
            px = player.position.x,
            py = player.position.y,
            pz = player.position.z,
            hearts = ph ? ph.CurrentHearts : 3,
            maxHearts = ph ? ph.MaxHearts : 3,
        };

        SaveSystem.Save(data);
        Debug.Log("[SavePoint] Saved: " + data.scene + $" ({data.px:F2},{data.py:F2}) hearts={data.hearts}/{data.maxHearts}");
    }
}
