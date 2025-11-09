using UnityEngine;

public class DebugUnlocker : MonoBehaviour
{
    [Header("Panels")]
    public GameObject debugPanel;        // 디버그 탭 루트 (초기 비활성)
    public GameObject devHotkeysObject;  // DevTools 오브젝트(비활성 저장). 타입 참조 없음!

    [Header("Unlock")]
    public float holdSeconds = 1.5f;
    float holdTimer;
    bool unlocked;

    void Start()
    {
        if (debugPanel) debugPanel.SetActive(false);

        // 인스펙터에서 안 넣었으면 이름으로 탐색 (타입 미사용)
        if (!devHotkeysObject)
        {
            devHotkeysObject = GameObject.Find("DevTools");
        }
        if (devHotkeysObject) devHotkeysObject.SetActive(false);
    }

    void Update()
    {
        if (unlocked) return;

        bool chord =
            (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) &&
            (Input.GetKey(KeyCode.LeftShift)   || Input.GetKey(KeyCode.RightShift))   &&
            Input.GetKey(KeyCode.D);

        if (chord)
        {
            // 일시정지 중에도 작동해야 하므로 unscaledDeltaTime 사용
            holdTimer += Time.unscaledDeltaTime;
            if (holdTimer >= holdSeconds) Unlock();
        }
        else
        {
            holdTimer = 0f;
        }
    }

    void Unlock()
    {
        unlocked = true;
        if (debugPanel) debugPanel.SetActive(true);
        if (devHotkeysObject) devHotkeysObject.SetActive(true);
        // Debug.Log("DEBUG UNLOCKED");
    }
}
