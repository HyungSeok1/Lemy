using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 
/// 프로토타입 시절 존재하던 코드입니다.
/// 
/// </summary>
/// 

public class InteractionManager : Singleton<InteractionManager>
{
    protected override void Awake()
    {
        base.Awake();
    }

    public void PlayerSubmitPressed(InputAction.CallbackContext context)
    {
        // NPC 상호작용
        if (context.performed)
        {
            GameEventsManager.Instance.inputEvents.SubmitPressed();
        }
    }

    public void UISubmitPressed(InputAction.CallbackContext context)
    {

        // NPC 상호작용
        if (context.performed)
        {
            GameEventsManager.Instance.inputEvents.SubmitPressed();
        }
    }

}