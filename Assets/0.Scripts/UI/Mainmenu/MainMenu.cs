using UnityEngine;

/// <summary>
/// 메인메뉴 씬의 버튼들에 할당된 메서드들이 담긴 스크립트 파일입니다.
/// 
/// 버튼들의 부모 GameObject인 ButtonsParent에 붙어있습니다.
/// 
/// </summary>
public class MainMenu : MonoBehaviour
{
    [SerializeField] private MainMenuUIManager uiManager;
    [SerializeField] private SaveslotsPanel savePanel;

    private void Start()
    {
        GameStateManager.Instance.currentStateData = new StateData(-1, -1, -1);
        Player.Instance.playerInputController.EnableUIActionMap();
    }

    private void Update()
    {
    }

    public void Continue()
    {

    }

    public void StartGame()
    {
        uiManager.ShowPanel(savePanel.gameObject);
        savePanel.ActiveSaveloadSlots();
    }

    public void Settings()
    {

    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
