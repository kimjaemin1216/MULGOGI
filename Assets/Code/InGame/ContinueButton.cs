using UnityEngine;
using UnityEngine.SceneManagement;

public class ContinueButton : MonoBehaviour
{
    public GameObject noSavePopup; // 저장 없을 때 안내 UI (옵션)

    public void Continue()
    {
        if (!SaveSystem.Exists())
        {
            if (noSavePopup) noSavePopup.SetActive(true);
            Debug.Log("[Continue] 저장 파일이 없습니다.");
            return;
        }

        var data = SaveSystem.Load();
        if (string.IsNullOrEmpty(data.scene))
        {
            Debug.LogWarning("[Continue] 저장된 씬 정보가 없습니다.");
            return;
        }

        // 씬 로드 후 플레이어 상태 복원
        SceneManager.LoadSceneAsync(data.scene).completed += _ =>
        {
            var player = GameObject.FindWithTag("Player");
            if (!player)
            {
                Debug.LogWarning("[Continue] 태그 'Player' 오브젝트를 찾지 못했습니다.");
                return;
            }

            player.transform.position = new Vector3(data.px, data.py, data.pz);

            var hp = player.GetComponent<PlayerHealth>();
            if (hp)
            {
                if (data.maxHearts > 0 && data.maxHearts != hp.MaxHearts)
                    hp.SetMaxHearts(data.maxHearts, keepCurrentRatio: false);

                hp.SetCurrentHearts(Mathf.Max(1, data.hearts)); // 0이면 즉사 로드 방지
            }

            Debug.Log("[Continue] 로드 완료.");
        };
    }
}
