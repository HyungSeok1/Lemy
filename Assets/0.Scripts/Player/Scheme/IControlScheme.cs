using UnityEngine.InputSystem;

/// <summary>
/// Player의 조작법을 모듈화하기 위한 인터페이스입니다.
/// 
/// IControlScheme 상속하여 각 메서드만 잘 구현해주시면 됩니다.
/// 
/// 구현 예시는 BasicScheme 참고해주세요.
/// </summary>
public interface IControlScheme
{
    SchemeData schemeData { get; }

    void OnSchemeSelected();
    void OnSchemeUnselected();

    void OnLeftClickStarted(InputAction.CallbackContext context);
    void OnLeftClickPerformed(InputAction.CallbackContext context);
    void OnLeftClickCanceled(InputAction.CallbackContext context);

    void OnRightClickStarted(InputAction.CallbackContext context);
    void OnRightClickPerformed(InputAction.CallbackContext context);
    void OnRightClickCanceled(InputAction.CallbackContext context);
}

