using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider BGMSlider;
    [SerializeField] private Slider sfxSlider;

    public void SetMasterVolume(float value)
    {
        SoundManager.Instance.SetMasterVolume(value);
    }

    public void SetMusicVolume(float value)
    {
        SoundManager.Instance.SetBGMVolume(value);
    }

    public void SetSFXVolume(float value)
    {
        SoundManager.Instance.SetSFXVolume(value);
    }
}
