using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public float masterVolume;
    public float BGMVolume;
    public float VFXVolume;
    public bool cameraShakeEnabled;


    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    // TODO: 슬라이더에 따라 값 변경 (소리 3종)

    public void ToggleCameraShake() => cameraShakeEnabled = !cameraShakeEnabled;
}
