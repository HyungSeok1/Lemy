using System.Collections.Generic;
using UnityEngine;

public class MainMenuUIManager : MonoBehaviour
{
    private Stack<GameObject> panelStack = new Stack<GameObject>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleBackAction();
        }
    }

    public void ShowPanel(GameObject panel)
    {
        panel.SetActive(true);
        panelStack.Push(panel);
    }

    public void HideCurrentPanel()
    {
        if (panelStack.Count > 0)
        {
            var top = panelStack.Pop();
            top.SetActive(false);
        }
        else
        {
            Debug.Log("No panels left. Maybe exit or ignore ESC.");
        }
    }

    public void HandleBackAction()
    {
        HideCurrentPanel();
    }
}
