using UnityEngine;
using UnityEngine.SceneManagement;





/// <summary>
/// 게임 State를 관리하는 클래스입니다.
/// 
/// PersistentSingleton이므로 GameStateManager.Instance. 로 접근 가능합니다.
/// 
/// 앞으로 구현 시 FSM으로 구현 예정입니다.
/// 
/// 
/// </summary>
/// 
public class GameStateManager : PersistentSingleton<GameStateManager>, ISaveable<StateData>
{

    /// <summary>
    /// 게임의 전역 상태를 정의하는 enum
    /// 게임이 현재 메뉴 화면인지, 플레이 중인지, 씬 로딩 중인지, 일시정지 상태인지를 구분
    /// </summary>
    public enum GameState
    {
        MainMenu,
        Playing,
        LoadingScene,
        Paused
    }

    protected override void Awake()
    {
        base.Awake();

        CurrentState = GameState.MainMenu;

        SceneManager.sceneLoaded += HandleSceneLoaded;
        ChangeChapterAndStage(-1, -1, -1);
    }

    public void ChangeChapterAndStage(int targetChapter, int targetMap, int targetNumber)
    {
        currentChapter = targetChapter;
        currentMap = targetMap;
        currentNumber = targetNumber;
    }

    #region 게임의 전역 상태 (GameState) FSM
    public int currentChapter;
    public int currentMap;
    public int currentNumber;
    public GameState CurrentState { get; private set; }

    //임시
    public void ChangeState(GameState newState)
    {
        CurrentState = newState;
    }
    #endregion

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 필요하면 현재 챕터/스테이지 로깅
        Debug.Log($"[GSM] scene loaded: {scene.name} (chapter:{currentChapter}, map:{currentMap})");
        if (currentMap / 100 == 1)
        {
            // 🔊 BGM 시작
            SoundManager.Instance.PlayBGM("gwanmoon_bgm", 0.5f);
        }
    }

    public void Save(ref StateData data)
    {
        data.chapter = currentChapter;
        data.map = currentMap;
        data.number = currentNumber;
    }

    public void Load(StateData data)
    {
        currentChapter = data.chapter;
        currentMap = data.map;
        currentNumber = data.number;
    }
}
