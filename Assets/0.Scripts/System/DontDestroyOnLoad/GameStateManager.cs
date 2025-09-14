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

        CurrentGameState = GameState.MainMenu;

        SceneManager.sceneLoaded += HandleSceneLoaded;

        // 메인메뉴 안에서 -1로 초기화 (빌드본에서만.)
#if !UNITY_EDITOR
    UpdateStateData(new StateData(-1, -1, -1));
#endif
    }

    public void UpdateStateData(StateData stateData)
    {
        currentStateData = stateData;
    }

    #region 게임의 전역 상태 (GameState) FSM

    public StateData currentStateData;

    public GameState CurrentGameState { get; private set; }

    //임시
    public void ChangeGameState(GameState newState)
    {
        CurrentGameState = newState;
    }
    #endregion

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 필요하면 현재 챕터/스테이지 로깅
        if (currentStateData.map / 100 == 1)
        {
            SoundManager.Instance.PlayBGM("yeonok_bgm", 1f);
        }
    }

    public void Save(ref StateData data)
    {
        data = currentStateData;
    }

    public void Load(StateData data)
    {
        currentStateData = data;
    }
}
