using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI: MonoBehaviour
{
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        masterSlider.value = AudioManager.Instance.masterVolume;
        musicSlider.value = AudioManager.Instance.musicVolume;
        sfxSlider.value = AudioManager.Instance.sfxVolume;
    }

    public void SetMasterVolume()
    {
        AudioManager.Instance.masterVolume = masterSlider.value;
    }

    public void SetMusicVolume()
    {
        AudioManager.Instance.musicVolume = musicSlider.value;
    }

    public void SetSFXVolume()
    {
        AudioManager.Instance.sfxVolume = sfxSlider.value;
    }
}
