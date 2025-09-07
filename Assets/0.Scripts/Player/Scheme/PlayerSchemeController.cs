using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;
using System;

/// <summary>
/// Player 오브젝트 아래에 붙어있는, SchemeManager 오브젝트에 컴포넌트로 붙여지는 스크립트입니다. 
/// 
/// schemes LIst 초기화 및 Input System 세팅을 담당합니다.
/// 
/// IControlScheme을 상속한 조작법 스크립트 파일들을 SchemeManager에 컴포넌트로 같이 붙여주시면 됩니다.
/// 
/// </summary>
public class PlayerSchemeController : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private SchemeDatabase schemeDatabase;

    // 왼쪽 클릭 핸들러
    private Action<InputAction.CallbackContext> onLeftStarted;
    private Action<InputAction.CallbackContext> onLeftPerformed;
    private Action<InputAction.CallbackContext> onLeftCanceled;

    // 오른쪽 클릭 핸들러
    private Action<InputAction.CallbackContext> onRightStarted;
    private Action<InputAction.CallbackContext> onRightPerformed;
    private Action<InputAction.CallbackContext> onRightCanceled;

    public List<IControlScheme> learnedSchemes = new();
    public IControlScheme currentScheme;

    void Start()
    {
        
    }

    /// Scheme 변경될때 호출되는 메서드입니다.
    /// Player같은곳에 이벤트같은거 등록/해제 필요하면 OnSchemeSelected에 넣어주시면 이 메서드에서 등록/해제됩니다.
    public void ChangeScheme(IControlScheme schemeRef)
    {
        if (currentScheme is not null)
            currentScheme.OnSchemeUnselected(); // 현재 스킴이 있다면 선택 해제합니다.

        currentScheme = schemeRef; // schemeRef를 현재 스킴으로 설정합니다.
        schemeRef.OnSchemeSelected();
        UnbindAndRebind(); // (이 안에서 currentScheme 접근함)
    }

    // currentScheme이 변경될 때마다 Input System의 바인딩을 새로 설정합니다.
    private void UnbindAndRebind()
    {
        // 좌클릭 바인딩 해제 및 등록
        var leftClick = playerInput.actions.FindActionMap("Player").FindAction("Left Click");
        // 1. 기존 구독 해제
        leftClick.started -= onLeftStarted;
        leftClick.performed -= onLeftPerformed;
        leftClick.canceled -= onLeftCanceled;
        // 2. 델리게이트 참조에 람다 할당 (나중에 이걸로 등록 해제하려고)
        onLeftStarted = ctx => currentScheme.OnLeftClickStarted(ctx);
        onLeftPerformed = ctx => currentScheme.OnLeftClickPerformed(ctx);
        onLeftCanceled = ctx => currentScheme.OnLeftClickCanceled(ctx);
        // 3. 새로 구독 추가 (저장한 참조만 등록)
        leftClick.started += onLeftStarted;
        leftClick.performed += onLeftPerformed;
        leftClick.canceled += onLeftCanceled;


        // 우클릭 바인딩 해제 및 등록
        var rightClick = playerInput.actions.FindActionMap("Player").FindAction("Right Click");
        // 1) 기존 구독 해제
        rightClick.started -= onRightStarted;
        rightClick.performed -= onRightPerformed;
        rightClick.canceled -= onRightCanceled;
        // 2) 델리게이트 참조에 람다 할당
        onRightStarted = ctx => currentScheme.OnRightClickStarted(ctx);
        onRightPerformed = ctx => currentScheme.OnRightClickPerformed(ctx);
        onRightCanceled = ctx => currentScheme.OnRightClickCanceled(ctx);
        // 3) 새로 구독 추가
        rightClick.started += onRightStarted;
        rightClick.performed += onRightPerformed;
        rightClick.canceled += onRightCanceled;
    }

    public void GetScheme(string schemeName)
    {
        var schemeRef = schemeDatabase.AddSchemeComponent(gameObject, schemeName);
        learnedSchemes.Add(schemeRef);

        if (learnedSchemes.Count == 1)
        {
            ChangeScheme(schemeRef);
            currentScheme = schemeRef;
        }
    }
}




