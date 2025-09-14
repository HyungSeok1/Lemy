using UnityEngine;


public class Pause : PersistentSingleton<Pause>
{
    [SerializeField] private UIStacker uiStacker;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settings;
    [SerializeField] private GameObject backToMenuWarner;
    [SerializeField] private GameObject exitGameWarner;


    public bool isPaused = false;

    protected override void Awake()
    {
        base.Awake();
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            Player.Instance.playerInputController.EnablePlayerActionMap();
            uiStacker.HideCurrentPanel();
            isPaused = !isPaused;
            Time.timeScale = 1f;
            GameStateManager.Instance.ChangeGameState(GameStateManager.GameState.Playing);
            return;
        }

        Player.Instance.playerInputController.EnableUIActionMap();
        uiStacker.ShowPanel(pauseMenu);
        isPaused = !isPaused;
        GameStateManager.Instance.ChangeGameState(GameStateManager.GameState.Paused);
        Time.timeScale = 0f;
    }

    public void EnableSettings()
    {
        uiStacker.ShowPanel(settings);
    }

    public void OnBackToMenuPressed()
    {
        uiStacker.ShowPanel(backToMenuWarner, null, false);
    }

    public void OnExitGamePressed()
    {
        uiStacker.ShowPanel(exitGameWarner, null, false);
    }

    // 실제 로직들
    public void BackToMenu()
    {
        SceneTransitionManager.Instance.StartLoadMainMenuWithFade();
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
