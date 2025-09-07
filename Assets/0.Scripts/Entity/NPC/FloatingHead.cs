using UnityEngine;

public class FloatingHead : MonoBehaviour, ICameraFollowable
{
    private void Start()
    {
        RegisterToSwitchTarget();
    }

    private void OnDisable()
    {
        UnRegisterToSwitchTarget();
    }

    public void RegisterToSwitchTarget()
    {
        MainCameraScript.Instance.RegisterSwitchTarget(this.gameObject);
    }

    public void UnRegisterToSwitchTarget()
    {
        if (MainCameraScript.Instance != null)
            MainCameraScript.Instance.RegisterSwitchTarget(this.gameObject);
    }
}

