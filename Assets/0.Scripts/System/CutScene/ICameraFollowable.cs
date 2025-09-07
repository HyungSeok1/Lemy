/// <summary>
/// 반드시 Start, OnEnable에 등록/해제 메서드 넣어줘야 함.
/// </summary>
public interface ICameraFollowable
{
    /// <summary>
    /// MaincameraScript의 switchTargetList에 등록하기 위해 존재하는 메서드.
    /// Start에서 등록하고, OnDisable에서 빼면 된다.
    /// </summary>
    public void RegisterToSwitchTarget();

    /// <summary>
    /// 마찬가지로 빼려는 것
    /// </summary>
    public void UnRegisterToSwitchTarget();
}