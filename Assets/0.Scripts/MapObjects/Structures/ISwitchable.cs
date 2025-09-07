using UnityEngine;

public interface ISwitchable
{
    bool IsOn { get; }
    void SetOn(bool on);
    void Toggle() => SetOn(!IsOn);
}
