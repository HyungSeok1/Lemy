using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 추후 수정 필요. 현재는 그냥 일단 강제로 꺼버리고 1프레임 뒤에 키고싶은걸 킴.
/// </summary>
public class PlayerInputController : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    public event Action OnESCPressed;

    private void Start()
    {
        Player.Instance.playerInputController.BindActionCallback("Global", "Pause", ctx => OnESCPressed?.Invoke());
    }

    public void BindActionCallback(string actionMapName, string actionName, Action<InputAction.CallbackContext> callback)
    {
        var actionMap = playerInput.actions.FindActionMap(actionMapName);
        if (actionMap == null)
        {
            Debug.LogError($"액션맵 '{actionMapName}'을 찾을 수 없습니다.");
            return;
        }

        var action = actionMap.FindAction(actionName);
        if (action == null)
        {
            Debug.LogError($"액션 '{actionName}'을 액션맵 '{actionMapName}'에서 찾을 수 없습니다.");
            return;
        }

        // 콜백 바인딩[170]
        action.performed += callback;
    }

    /// <summary>
    /// Global 맵만 켜기
    /// </summary>
    public void EnableGlobalOnly()
    {
        foreach (var map in playerInput.actions.actionMaps)
        {
            if (map.name == "Global")
            {
                map.Enable();
                continue;               // Global은 건드리지 않는다
            }
            map.Disable();             // Global 외엔 전부 끈다
        }
    }

    public void EnableUIActionMap()
    {
        EnableThisActionMap("UI");
    }


    public void EnablePlayerActionMap()
    {
        EnableThisActionMap("Player");
    }

    /// <summary>
    /// 모든 맵 끄기
    /// </summary>
    public void DisableAllActionMap()
    {
        foreach (var map in playerInput.actions.actionMaps)
            map.Disable();
    }

    /// <summary>
    /// Global 맵을 제외한 모든 액션맵을 끄고, 지정한 액션맵만 켭니다.
    /// </summary>
    /// <param name="mapToEnable"></param>
    private void EnableThisActionMap(string mapToEnable)
    {
        foreach (var map in playerInput.actions.actionMaps)
        {
            if (map.name == "Global")
                continue;               // Global은 건드리지 않는다
            map.Disable();             // Global 외엔 전부 끈다
        }

        var target = playerInput.actions.FindActionMap(mapToEnable);
        if (target != null)
            target.Enable();          // 지정한 맵만 켠다
        else
            Debug.LogError($"맵 '{mapToEnable}'을 찾을 수 없습니다.");
    }



    public void CheckActionMapsStatus()
    {
        // 확인할 액션맵 목록
        string[] actionMapNames = { "Player", "UI", "Global" };

        StringBuilder names = new StringBuilder();
        StringBuilder statuses = new StringBuilder();

        foreach (string mapName in actionMapNames)
        {
            var actionMap = playerInput.actions.FindActionMap(mapName);

            if (actionMap != null)
            {
                names.Append(mapName).Append(" / ");
                statuses.Append(actionMap.enabled ? "true" : "false").Append(" / ");
            }
            else
            {
                names.Append(mapName).Append(" / ");
                statuses.Append("NotFound").Append(" / ");
            }
        }

        // 마지막 구분자 제거
        if (names.Length > 3) names.Length -= 3;
        if (statuses.Length > 3) statuses.Length -= 3;

        Debug.Log($"{names}\n{statuses}");
    }

}
