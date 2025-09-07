using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    private PlayerInput playerInput;
    // 액션맵 조작

    private void Start()
    {
        playerInput = Player.Instance.playerInput;
    }

    /// <summary>
    /// Global 맵만 켜기
    /// </summary>
    public void TurnOnGlobalOnly()
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
        TurnOnThisActionMap("UI");
    }


    public void EnablePlayerActionMap()
    {
        TurnOnThisActionMap("Player");
    }

    /// <summary>
    /// 모든 맵 끄기
    /// </summary>
    public void TurnOffActionMap()
    {
        foreach (var map in playerInput.actions.actionMaps)
            map.Disable();
    }

    /// <summary>
    /// Global 맵을 제외한 모든 액션맵을 끄고, 지정한 액션맵만 켭니다.
    /// </summary>
    /// <param name="mapToEnable"></param>
    private void TurnOnThisActionMap(string mapToEnable)
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

  
}
