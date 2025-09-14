using UnityEngine;

/// <summary>
/// 인게임에서 ESC 누르면 나오는 옵션
/// </summary>
public class IngameSettings : MonoBehaviour
{
    public void OnMasterVolumeChanged(float value)
    {
        SoundManager.Instance.SetMasterVolume(value);
    }

    public void OnBGMVolumeChanged(float value)
    {
        SoundManager.Instance.SetBGMVolume(value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        SoundManager.Instance.SetSFXVolume(value);
    }

    public void OnCameraShakeIntensityChanged(float value)
    {
        SettingsHub.Instance.cameraShakeIntensity = value;
    }
}
