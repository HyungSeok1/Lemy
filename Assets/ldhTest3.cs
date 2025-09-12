using UnityEngine;
using UnityEngine.InputSystem;

public class ldhTest3 : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;

    private void Awake()
    {
        CheckActionMapsStatus();
    }

    private void Start()
    {
        CheckActionMapsStatus();
    }

    private void CheckActionMapsStatus()
    {
        // 현재 활성화된 Action Map 확인
        Debug.Log($"Current Action Map: {playerInput.currentActionMap.name}");

        // 각 Action Map의 활성화 상태 확인
        CheckActionMapEnabled("Player");
        CheckActionMapEnabled("UI");
        CheckActionMapEnabled("Global");
    }

    private void CheckActionMapEnabled(string actionMapName)
    {
        // Action Map 찾기
        var actionMap = playerInput.actions.FindActionMap(actionMapName);

        if (actionMap != null)
        {
            // 활성화 상태 확인
            bool isEnabled = actionMap.enabled;
            Debug.Log($"{actionMapName} Action Map: {(isEnabled ? "Enabled" : "Disabled")}");
        }
        else
        {
            Debug.LogWarning($"{actionMapName} Action Map을 찾을 수 없습니다.");
        }
    }
}
