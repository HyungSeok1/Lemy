using UnityEngine.InputSystem;
using UnityEngine;


/// <summary>
/// 기존에 존재하던 기본 조작의 모듈화된 형태입니다.
/// 
/// 이 구조를 참고하셔서 다른 조작법도 만들어주시면 될 것 같습니다.
/// 
/// </summary>


public class BasicScheme : MonoBehaviour, IControlScheme
{
    public string schemeName => "BasicScheme";


    public SchemeData schemeData => data;
    [SerializeField] public SchemeData data;

    public void OnSchemeSelected()
    {
       // Player.Instance.currentSchemeAction += Player.Instance.BasicSchemeAction;
    }

    public void OnSchemeUnselected()
    {
     //   Player.Instance.currentSchemeAction -= Player.Instance.BasicSchemeAction;
    }

    public void OnLeftClickStarted(InputAction.CallbackContext context)
    {
     //   Player.Instance.isLeftHeld = true;
     //   Player.Instance.StartMovement(); // 마우스 클릭 시작 시 이동 시작
    }

    public void OnLeftClickPerformed(InputAction.CallbackContext context)
    {
        return;
    }

    public void OnLeftClickCanceled(InputAction.CallbackContext context)
    {
     //   Player.Instance.isLeftHeld = false;
    }

    public void OnRightClickStarted(InputAction.CallbackContext context)
    {

    }

    public void OnRightClickPerformed(InputAction.CallbackContext context)
    {
        return;
    }

    public void OnRightClickCanceled(InputAction.CallbackContext context)
    {
       
    }

}
