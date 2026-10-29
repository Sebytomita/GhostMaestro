using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    public AudioMixer masterMixer; 
    public Slider volumeSlider;    

    const string VolKey = "MASTER_VOL";
    const string VolParam = "MasterVolume";

    void OnEnable()
    {
        float v = PlayerPrefs.GetFloat(VolKey, 0.8f);
        if (volumeSlider) volumeSlider.value = v;
        ApplyVolume(v);
    }

    public void OnVolumeChanged(float v)
    {
        ApplyVolume(v);
        PlayerPrefs.SetFloat(VolKey, v);
    }

    void ApplyVolume(float v)
    {
        float db = (v <= 0.0001f) ? -80f : Mathf.Log10(v) * 20f;
        masterMixer.SetFloat(VolParam, db);
    }
}
