using Unity.Cinemachine;
using UnityEngine;


/// <summary>
/// 이 스크립트가 붙어있는 시네머신 카메라를 CameraSwitcher에 등록하는 클래스입니다.
/// 
/// </summary>
public class CameraResigter : MonoBehaviour
{
    private void OnEnable()
    {
        CameraSwitcher.Register(GetComponent<CinemachineCamera>());
    }

    private void OnDisable()
    {
        CameraSwitcher.Unregister(GetComponent<CinemachineCamera>());

    }
}
