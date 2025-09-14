using DG.Tweening;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
/// <summary>
/// 
/// 세이브로드, 씬 로드, 페이드, GameState전환 등 해주는 스크립트입니다
/// 
/// 씬 전환 시 1. 페이드 아웃 → 2. 비동기 로드(LoadSceneAsync) → 3. 페이드 인 순으로 화면 전환 효과를 적용
/// 4. PlayerData 데이터 적용(위치, 챕터, 스테이지 등)
/// 5. GameState 적용 (Playing)
/// 
/// </summary>

public class SceneTransitionManager : PersistentSingleton<SceneTransitionManager>
{
    private string targetScene;

    [SerializeField] private CanvasGroup fadeOverlayCanvas; // canvas space - camera ,  overlayCamera에만 보임
    [SerializeField] private CanvasGroup fadeCanvas; // canvas space - overlay
    [SerializeField] private float fadeDuration;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    // TODO: PendingLoad: 아마 예전에 씬전환후 데이터 넣어주는 용도였던거같은데, 구조 수정해야 할듯.
    public void StartTransition(StateData stateData, PositionData positionData, Action callback)
    {
        StartCoroutine(TransitionCoroutine(stateData, positionData, callback));
    }

    // 진짜 씬 바꿔주고 State 바꿔주는 것만 함.
    public IEnumerator TransitionCoroutine(StateData stateData, PositionData positionData, Action callback = null)
    {
        targetScene = $"Scene{stateData.chapter}_{stateData.chapter}_{stateData.number}";

        GameStateManager.Instance.ChangeGameState(GameStateManager.GameState.LoadingScene);

        // 기다리기 
        AsyncOperation load = SceneManager.LoadSceneAsync(targetScene);
        while (load != null && !load.isDone)
        {
            yield return null;
        }

        GameStateManager.Instance.ChangeGameState(GameStateManager.GameState.Playing);
        GameStateManager.Instance.UpdateStateData(stateData); // stateData 적용
        Player.Instance.Load(positionData);

        callback?.Invoke();
    }


    #region Portal 전용 Transition

    public void StartPortalTransition(string targetScene, string entranceID)
    {
        StartCoroutine(PortalTransitionCoroutine(targetScene, entranceID));
    }

    public IEnumerator PortalTransitionCoroutine(string targetScene, string entranceID)
    {
        // 화면을 어둡게
        yield return FadeOut();

        GameStateManager.Instance.ChangeGameState(GameStateManager.GameState.LoadingScene);

        // 로드 완료 시 진입 지점으로 텔레포트
        UnityAction<Scene, LoadSceneMode> onLoaded = null;
        onLoaded = (scene, mode) => { FindPortalAndTelePort(entranceID); };
        SceneManager.sceneLoaded += onLoaded;

        // 기다리기 
        AsyncOperation load = SceneManager.LoadSceneAsync(targetScene);
        while (load != null && !load.isDone)
        {
            yield return null;
        }

        SceneManager.sceneLoaded -= onLoaded;

        GameStateManager.Instance.ChangeGameState(GameStateManager.GameState.Playing);

        // 화면을 밝게
        yield return FadeIn();
    }

    private void FindPortalAndTelePort(string entranceID)
    {
        // 없으면 아무것도 안하고 리턴 
        if (string.IsNullOrEmpty(entranceID))
        {
            Debug.LogError("포탈 번호가 지정이 안 됨..");
            return;
        }
        // 모든 PortalExit을 검색
        PortalExit[] exits = FindObjectsByType<PortalExit>(FindObjectsSortMode.None);

        // targetID와 일치하는 exit들만 필터링
        var matched = exits.Where(e => e.exitID == entranceID).ToArray();
        int count = matched.Length;

        if (count == 0)
            Debug.LogError($"[SceneTransitionManager] '{entranceID}'에 매칭되는 PortalExit을 찾지 못했습니다.");
        else if (count > 1)
            Debug.LogError($"[SceneTransitionManager] '{entranceID}'에 매칭되는 PortalExit이 {count}개 존재합니다. 반드시 1개만 존재해야 합니다.");
        else
        {
            var exit = matched[0]; //portalExit형 인스턴스
            if (Player.Instance != null)
            {
                Player.Instance.transform.position = exit.transform.position;

                CameraSwitcher.ActiveCamera.Follow = Player.Instance.transform; // 이 코드 끼워넣음 - 재용

                var cinemachineCam = MainCameraScript.Instance.GetActiveCinemachineCam();
                cinemachineCam.ForceCameraPosition(Player.Instance.transform.position, cinemachineCam.transform.rotation);
            }
            else
                Debug.LogError("[SceneTransitionManager] PlayerController 인스턴스를 찾을 수 없습니다.");
        }
    }

    #endregion


    #region MainMenu

    public void StartLoadMainMenuWithFade()
    {
        StartCoroutine(LoadMainMenuWithFade());
    }

    public IEnumerator LoadMainMenuWithFade()
    {
        yield return FadeInRealtime();
        yield return LoadMainMenuCoroutine();
        yield return FadeOutRealtime();
    }

    public IEnumerator LoadMainMenuCoroutine()
    {
        targetScene = $"MainMenu";

        GameStateManager.Instance.ChangeGameState(GameStateManager.GameState.LoadingScene);

        // 기다리기 
        AsyncOperation load = SceneManager.LoadSceneAsync(targetScene);
        while (load != null && !load.isDone)
        {
            yield return null;
        }

        GameStateManager.Instance.ChangeGameState(GameStateManager.GameState.MainMenu);
    }
    #endregion

    #region Fade 모음
    //어두워짐
    public IEnumerator OverlayFadeOut()
    {
        // 0f에서 시작
        fadeOverlayCanvas.DOFade(1f, fadeDuration).SetEase(Ease.Linear).SetUpdate(true);
        yield return new WaitForSeconds(fadeDuration);
    }

    // 밝아짐
    public IEnumerator OverlayFadeIn()
    {
        // 1f에서 시작
        fadeOverlayCanvas.DOFade(0f, fadeDuration).SetEase(Ease.Linear).SetUpdate(true);
        yield return new WaitForSeconds(fadeDuration);
    }

    // 밝아짐
    public IEnumerator FadeIn()
    {
        // 0f에서 시작
        fadeCanvas.DOFade(0f, fadeDuration).SetEase(Ease.Linear).SetUpdate(true);
        yield return new WaitForSeconds(fadeDuration);
    }

    public IEnumerator FadeOut()
    {
        // 1f에서 시작
        fadeCanvas.DOFade(1f, fadeDuration).SetEase(Ease.Linear).SetUpdate(true);
        yield return new WaitForSeconds(fadeDuration);
    }

    public void StartTransitionWithFade(StateData stateData, PositionData positionData)
    {
        StartCoroutine(TransitionCoroutine(stateData, positionData));
    }

    public IEnumerator TransitionWithFade(StateData stateData, PositionData positionData)
    {
        yield return OverlayFadeIn();
        yield return TransitionCoroutine(stateData, positionData);
        yield return OverlayFadeOut();
    }

    public IEnumerator FadeOutRealtime()
    {
        // 0f에서 시작
        fadeCanvas.DOFade(1f, fadeDuration).SetEase(Ease.Linear).SetUpdate(true);
        yield return new WaitForSecondsRealtime(fadeDuration);
    }

    public IEnumerator FadeInRealtime()
    {
        // 1f에서 시작
        fadeCanvas.DOFade(0f, fadeDuration).SetEase(Ease.Linear).SetUpdate(true);
        yield return new WaitForSecondsRealtime(fadeDuration);
    }

    #endregion

}
