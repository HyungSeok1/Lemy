using UnityEngine;

/// <summary>
/// 추후에 상호작용 가능한 구조물이 나올 시 그 클래스에 구현할 인터페이스입니다.
/// 
/// 
/// </summary>
public interface IInteractible 
{
    bool CanInteract { get; }
    float InteractionRange { get; }


    void Interact();

    void OnEnterRange();
    void OnExitRange();
}
