using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

public class ldhTest3 : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerInputController controller;


    public bool a, b, c, d, e;
    private void Awake()
    {
        CheckActionMapsStatus();
        if (c)
            controller.EnableGlobalOnly();
        CheckActionMapsStatus();
    }

    private void Start()
    {
        CheckActionMapsStatus();
        if (d)
            controller.EnableGlobalOnly();

        CheckActionMapsStatus();

        StartCoroutine(asdf());
    }


    private IEnumerator asdf()
    {
        yield return null;
        if (e) controller.EnableGlobalOnly();
        CheckActionMapsStatus();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
            CheckActionMapsStatus();
    }

    private void CheckActionMapsStatus()
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
