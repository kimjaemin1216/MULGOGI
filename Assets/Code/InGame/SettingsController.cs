using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsController : MonoBehaviour
{
    [Header("Audio (optional)")]
    public AudioMixer mixer;            // 없으면 비워두기
    public Slider masterSlider;         // 0~1
    public Slider musicSlider;          // 0~1
    public Slider sfxSlider;            // 0~1

    const string MASTER = "MasterVol";
    const string MUSIC  = "MusicVol";
    const string SFX    = "SfxVol";

    void Start()
    {
        // 초기값
        if (masterSlider) masterSlider.onValueChanged.AddListener(SetMaster);
        if (musicSlider)  musicSlider.onValueChanged.AddListener(SetMusic);
        if (sfxSlider)    sfxSlider.onValueChanged.AddListener(SetSfx);
    }

    static float ToDb(float v) => Mathf.Clamp(Mathf.Log10(Mathf.Max(0.0001f, v)) * 20f, -80f, 0f);

    public void SetMaster(float v)
    {
        if (mixer) mixer.SetFloat(MASTER, ToDb(v));
        else AudioListener.volume = v;
    }
    public void SetMusic(float v)
    {
        if (mixer) mixer.SetFloat(MUSIC, ToDb(v));
    }
    public void SetSfx(float v)
    {
        if (mixer) mixer.SetFloat(SFX, ToDb(v));
    }
}
