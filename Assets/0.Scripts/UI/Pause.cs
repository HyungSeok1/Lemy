using UnityEngine;


public class Pause : PersistentSingleton<Pause>
{
    [SerializeField] private UIStacker uiStacker;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settings;
    [SerializeField] private GameObject backToMenuWarner;
    [SerializeField] private GameObject exitGameWarner;


    private bool isPaused = false;

    protected override void Awake()
    {
        base.Awake();
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            uiStacker.HideCurrentPanel();
            isPaused = !isPaused;
            Time.timeScale = 1f;
            GameStateManager.Instance.ChangeGameState(GameStateManager.GameState.Playing);
            return;
        }

        uiStacker.ShowPanel(pauseMenu);
        isPaused = !isPaused;
        GameStateManager.Instance.ChangeGameState(GameStateManager.GameState.Paused);
        Time.timeScale = 0f;
    }

    public void EnableSettings()
    {
        uiStacker.ShowPanel(settings);
    }

    public void BackToMenu()
    {
        uiStacker.ShowPanel(backToMenuWarner, null, false);
    }

    public void ExitGame()
    {
        uiStacker.ShowPanel(exitGameWarner, null, false);
    }
}
