using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System;

public class CutsceneManager : PersistentSingleton<CutsceneManager>
{
    [SerializeField] PlayableDirector director;

    public event Action OnTimelineChanged;
    [HideInInspector] public bool isCutscenePlaying = false;
    protected override void Awake()
    {
        base.Awake();
    }

    void OnEnable()
    {
        director.stopped += OnTimelineStopped;
    }

    void OnDisable()
    {
        director.stopped -= OnTimelineStopped;
    }

    public void PlayCutscene(TimelineAsset cutscene)
    {
        isCutscenePlaying = true;

        if (cutscene == null)
            Debug.LogError("cutscene이 null");


        director.playableAsset = cutscene;
        director.RebuildGraph();
        OnTimelineChanged.Invoke(); // 바인딩

        var receiver = GetComponent<CameraSwitchReceiver>();

        director.Play();

    }

    private void OnTimelineStopped(PlayableDirector aDirector)
    {
        isCutscenePlaying = false;
    }

    /// <summary>
    /// 특정 이름의 트랙을 찾아서, 그 트랙에 게임오브젝트를 바인딩함.
    /// </summary>
    /// <param name="obj">바인딩할 게임오브젝트</param>
    /// <param name="trackName">찾을 트랙 이름 시작 문자열</param>
    public void BindByTrackName(GameObject obj, string trackName)
    {
        if (director.playableAsset is TimelineAsset timeline)
        {
            foreach (var track in timeline.GetOutputTracks())
            {
                if (track.name != trackName) continue;

                switch (track)
                {
                    case AnimationTrack animationTrack:
                        var animator = obj.GetComponent<Animator>();
                        if (animator != null)
                            director.SetGenericBinding(track, animator);
                        else
                            Debug.LogWarning($"Animator가 {obj.name}에 없음!");
                        break;

                    case SignalTrack signalTrack:
                        var receiver = obj.GetComponent<SignalReceiver>();
                        if (receiver != null)
                            director.SetGenericBinding(track, receiver);
                        else
                            Debug.LogWarning($"SignalReceiver가 {obj.name}에 없음!");
                        break;

                    case ActivationTrack activationTrack:
                        if (obj != null)
                            director.SetGenericBinding(track, obj);
                        else
                            Debug.LogWarning($"{obj.name} 없음!");
                        break;

                    default:
                        Debug.LogError($"지원되지 않는 트랙 타입: {track.GetType()}");
                        break;
                }

            }
        }
    }
}
