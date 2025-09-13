using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;

/// <summary>
/// 메인메뉴 씬의 버튼들에 할당된 메서드들이 담긴 스크립트 파일입니다.
/// 
/// 버튼들의 부모 GameObject인 ButtonsParent에 붙어있습니다.
/// 
/// </summary>
public class MainMenu : MonoBehaviour
{
    [SerializeField] private UIStacker uiManager;

    [SerializeField] private SaveslotsPanel savePanel;
    [SerializeField] private TMP_Text continueText;
    [SerializeField] private SettingsPanel settingsPanel;

    [SerializeField] private float continueTargetAlpha;

    private PlayerInputController inputController;

    private bool continueAvailable = false;


    private void Start()
    {
        GameStateManager.Instance.currentStateData = new StateData(-1, -1, -1);

        inputController = Player.Instance.playerInputController;
        Player.Instance.GetComponent<SpriteRenderer>().enabled = false; 

        // 인풋 관련
        inputController.EnableGlobalOnly();
        inputController.CheckActionMapsStatus();
        StartCoroutine(DelayedStart());

        // 마지막으로 플레이한 슬롯정보 읽기 (Continue 버튼 활성화를 위해)
        string slot = File.ReadAllText(SaveLoadManager.Instance.GetLastSlotPath());
        string path = SaveLoadManager.Instance.GetMetaPathForSlot(int.Parse(slot));
        string json = File.ReadAllText(path);
        var metaData = JsonUtility.FromJson<SaveMeta>(json);
        if (metaData.isEmpty)
        {
            continueText.CrossFadeAlpha(continueTargetAlpha, 0, false); // 즉시 알파 0.5로 변경
            continueAvailable = false;
        }
        else
        {
            continueAvailable = true;
        }

    }

    public void Continue()
    {
        if (continueAvailable)
        {
            string slot = File.ReadAllText(SaveLoadManager.Instance.GetLastSlotPath());
            SaveLoadManager.Instance.LoadGame(int.Parse(slot));
        }
    }

    public void StartGame()
    {
        uiManager.ShowPanel(savePanel.SlotParent.gameObject);
        savePanel.ActiveSaveloadSlots();
    }

    public void Settings()
    {
        uiManager.ShowPanel(settingsPanel.firstSettingsPanel.gameObject);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private IEnumerator DelayedStart()
    {
        yield return null;
        inputController.EnableUIActionMap();
        inputController.CheckActionMapsStatus();
    }

}
