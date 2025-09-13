using UnityEngine;


public class Pause : PersistentSingleton<Pause>
{
    [SerializeField] private UIStacker uiManager;

    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject backgroundImage;

    protected override void Awake()
    {
        base.Awake();
    }

    public void TogglePause()
    {
        backgroundImage.SetActive(true);

        uiManager.ShowPanel(pauseMenuUI, () => backgroundImage.SetActive(false));
    }
}
