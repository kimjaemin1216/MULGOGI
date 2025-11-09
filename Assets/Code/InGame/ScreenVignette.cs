using UnityEngine;
using UnityEngine.UI;

public class ScreenVignette : MonoBehaviour
{
    [SerializeField] private Image darkOverlay;   // 풀화면 검은 이미지
    [SerializeField] private float minAlpha = 0f; // 스택 0
    [SerializeField] private float maxAlpha = 0.7f; // 스택 최대

    public void SetBlindness(int stacks, int maxStacks)
    {
        if (darkOverlay == null) return;
        float t = (maxStacks == 0) ? 0f : Mathf.Clamp01((float)stacks / maxStacks);
        var c = darkOverlay.color;
        c.a = Mathf.Lerp(minAlpha, maxAlpha, t);
        darkOverlay.color = c;
    }
}
