using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;

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
    public PlayerData loadedData;

    [SerializeField] private CanvasGroup fadeCanvas;
    [SerializeField] private float fadeDuration = 1f;

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

        GameStateManager.Instance.ChangeState(GameStateManager.GameState.LoadingScene);

        // 기다리기 
        AsyncOperation load = SceneManager.LoadSceneAsync(targetScene);
        while (load != null && !load.isDone)
        {
            yield return null;
        }

        GameStateManager.Instance.ChangeState(GameStateManager.GameState.Playing);
        GameStateManager.Instance.UpdateStateData(stateData); // stateData 적용
        Player.Instance.transform.position = positionData.pos; // positionData 적용

        callback?.Invoke();
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    public IEnumerator FadeOutCoroutine()
    {
        // 0f에서 시작
        fadeCanvas.DOFade(1f, fadeDuration).SetEase(Ease.Linear).SetUpdate(true);
        yield return new WaitForSeconds(fadeDuration);
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInCoroutine());
    }

    public IEnumerator FadeInCoroutine()
    {
        // 1f에서 시작
        fadeCanvas.DOFade(0f, fadeDuration).SetEase(Ease.Linear).SetUpdate(true);
        yield return new WaitForSeconds(fadeDuration);
    }


    public void StartTransitionWithFade(StateData stateData, PositionData positionData)
    {
        StartCoroutine(TransitionCoroutine(stateData, positionData));
    }

    public IEnumerator TransitionWithFade(StateData stateData, PositionData positionData)
    {
        yield return FadeInCoroutine();
        yield return TransitionCoroutine(stateData, positionData);
        yield return FadeOutCoroutine();
    }

    #region Portal 전용 Transition

    public void StartPortalTransition(string targetScene, string entranceID)
    {
        StartCoroutine(PortalTransitionCoroutine(targetScene, entranceID));
    }

    public IEnumerator PortalTransitionCoroutine(string targetScene, string entranceID)
    {
        GameStateManager.Instance.ChangeState(GameStateManager.GameState.LoadingScene);

        SceneManager.sceneLoaded += (scene, mode) =>
        {
            FindPortalAndTelePort(entranceID);
        };

        // 기다리기 
        AsyncOperation load = SceneManager.LoadSceneAsync(targetScene);
        while (load != null && !load.isDone)
        {
            yield return null;
        }

        SceneManager.sceneLoaded -= (scene, mode) =>
        {
            FindPortalAndTelePort(entranceID);
        };

        GameStateManager.Instance.ChangeState(GameStateManager.GameState.Playing);
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
}
