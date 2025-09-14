using System;
using System.Collections.Generic;
using UnityEngine;

public class UIStacker : MonoBehaviour
{
    private Stack<(GameObject, Action)> panelStack = new Stack<(GameObject, Action)>();

    private void Start()
    {
        Player.Instance.playerInputController.OnESCPressed += ESCBehaviour;
    }

    private void ESCBehaviour()
    {
        if (panelStack.Count > 0)
        {
            HideCurrentPanel();
        }
        else
        {
            if (Pause.Instance != null)
                Pause.Instance.TogglePause();
        }
    }

    public void ShowPanel(GameObject panel, Action callback = null)
    {
        panel.SetActive(true);
        panelStack.Push((panel, callback));
    }

    public void HideCurrentPanel()
    {
        if (panelStack.Count > 0)
        {
            var top = panelStack.Pop();
            top.Item1.SetActive(false);
            top.Item2?.Invoke();
        }
    }

}

