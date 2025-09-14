using System;
using System.Collections.Generic;
using UnityEngine;

public class UIStacker : MonoBehaviour
{
    private Stack<(GameObject, Action)> panelStack = new Stack<(GameObject, Action)>();
    [SerializeField] private GameObject background; // 클릭 raycast 막혀있음

    private void Start()
    {
        Player.Instance.playerInputController.OnESCPressed += ESCBehaviour;
    }

    private void ESCBehaviour()
    {
        if (panelStack.Count > 1)
        {
            HideCurrentPanel();
        }
        else if (panelStack.Count == 1)
        {
            if (GameStateManager.Instance.CurrentGameState == GameStateManager.GameState.MainMenu)
                HideCurrentPanel();

            if (GameStateManager.Instance.CurrentGameState == GameStateManager.GameState.Paused)
                Pause.Instance.TogglePause();
        }
        else if (panelStack.Count == 0)
        {
            if (GameStateManager.Instance.CurrentGameState == GameStateManager.GameState.Playing)
                Pause.Instance.TogglePause();
        }
    }

    /// <summary>
    /// 예전에 callback 필요할까해서 넣었는데, 거의 안씀.
    /// </summary>
    /// <param name="panel"></param>
    /// <param name="callback"></param>
    public void ShowPanel(GameObject panel, Action callback = null, bool turnOffPrev = true)
    {
        if (panelStack.Count == 0)
            background.SetActive(true);

        panel.SetActive(true);
        panelStack.Push((panel, callback));

        if (panelStack.Count > 1 && turnOffPrev)
            panelStack.Peek().Item1.SetActive(false);
    }

    public void HideCurrentPanel()
    {
        if (panelStack.Count == 1)
            background.SetActive(false);

        var top = panelStack.Pop();
        top.Item1.SetActive(false);
        top.Item2?.Invoke();

        if (panelStack.Count > 0)
            panelStack.Peek().Item1.SetActive(true);
    }

}

