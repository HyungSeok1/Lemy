using DG.Tweening;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class MainCameraScript : PersistentSingleton<MainCameraScript>
{
    [SerializeField] public CinemachineCamera mainCinemachineCamera;
    [SerializeField] public Camera overlayCamera;

    [SerializeField] private float frequency;
    [SerializeField] private float shakeTime;


    [SerializeField] private CinemachineBasicMultiChannelPerlin multiChannelPerlin;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        CameraSwitcher.SwitchCamera(mainCinemachineCamera);

        multiChannelPerlin.FrequencyGain = 0;
    }

    public void ShakeCamera(float _)
    {
        // TODO: 추후에 시네머신카메라가 도중 교체될 가능성이 있다면, 방어 코드 작성 필요

        multiChannelPerlin.FrequencyGain = frequency * SettingsHub.Instance.cameraShakeIntensity;
        DOTween.To(
            () => multiChannelPerlin.FrequencyGain,
            x => multiChannelPerlin.FrequencyGain = x,
            0f,
            shakeTime
        ).SetEase(Ease.OutQuad);
    }

    public CinemachineCamera GetActiveCinemachineCam()
    {
        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
        var activeCam = brain.ActiveVirtualCamera;
        var liveCam = (CinemachineCamera)activeCam;
        return liveCam;
    }

    public List<GameObject> switchTargetList = new();

    [ContextMenu("Execute Follow Camera By Index")]
    public void FollowCameraByName(string name)
    {
        foreach (GameObject go in switchTargetList)
        {
            if (go.name == name)
            {
                var cam = GetActiveCinemachineCam();
                cam.Follow = go.transform;
                break;
            }
        }

    }

    public void FollowCamera(GameObject go)
    {
        mainCinemachineCamera.Follow = go.transform;
    }

    public void RegisterSwitchTarget(GameObject target)
    {
        if (!switchTargetList.Contains(target))
            switchTargetList.Add(target);
    }

    public void UnRegisterSwitchTarget(GameObject target)
    {
        if (switchTargetList.Contains(target))
            switchTargetList.Remove(target);
    }
}
