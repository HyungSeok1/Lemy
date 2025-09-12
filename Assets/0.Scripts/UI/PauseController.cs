using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;


public class PauseController : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button BackToMenuButton;
    [SerializeField] private Button quitButton;

    private bool isPaused = false;

    void Start()
    {
        if (gameObject.activeSelf) pauseMenuUI.SetActive(false);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenuUI.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void ContinueGame()
    {

    }

    public void ToggleOption()
    {

    }

    public void BackToMenu()
    {

    }

    public void QuitGame()
    {

    }
}
