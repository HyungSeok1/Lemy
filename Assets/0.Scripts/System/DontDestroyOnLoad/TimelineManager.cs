using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public enum TimelineID
{
    GetKey,
    Map1OpenDoor
    // 필요하면 계속 추가
}

public class TimelineManager : PersistentSingleton<TimelineManager>
{
    [SerializeField] private PlayableDirector director;

    [System.Serializable]
    private struct TimelineData
    {
        public TimelineID id;
        public PlayableAsset asset;
    }

    [SerializeField] private List<TimelineData> timelines;
    private Dictionary<TimelineID, PlayableAsset> timelineDict;

    // 타임라인 교체 이벤트
    public event System.Action<PlayableDirector, PlayableAsset> OnTimelineChanged;

    protected override void Awake()
    {
        base.Awake();
        timelineDict = new Dictionary<TimelineID, PlayableAsset>();

        foreach (var tl in timelines)
        {
            timelineDict[tl.id] = tl.asset;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.O))
        {
            PrintTimelineSubscribers();
        }
    }

    public void PlayTimeline(TimelineID id)
    {
        if (director.state == PlayState.Playing)
        {
            Debug.Log($"Timeline {id} ignored because another timeline is playing.");
            return;
        }

        if (timelineDict.TryGetValue(id, out var asset))
        {
            director.playableAsset = asset;

            // 액션 invoke. 이걸 다른 애들이 알아차리고 바인딩
            OnTimelineChanged?.Invoke(director, asset);

            director.Play();
        }
        else
        {
            Debug.LogWarning($"TimelineAsset {id} not found!");
        }
    }

    /// <summary>
    /// Player, CoverImage 등 항상 존재하는 애들은 여기서 바인딩... 했어야 하지만 그냥 무시. 바인딩 안됨.
    /// </summary>
    private void BindStaticTracks(PlayableDirector director)
    {
        var timeline = director.playableAsset as TimelineAsset;
        if (timeline == null) return;

        foreach (var track in timeline.GetOutputTracks())
        {
            switch (track.name)
            {
                case "Player":
                    var playerObj = GameObject.FindWithTag("Player");
                    if (playerObj != null)
                        director.SetGenericBinding(track, playerObj.GetComponent<Animator>());
                    break;

                case "CoverImage":
                    var coverObj = GameObject.Find("CoverImage");
                    if (coverObj != null)
                        director.SetGenericBinding(track, coverObj.GetComponent<Animator>());
                    break;

                case "TransitionLight":
                    var lightObj = GameObject.Find("TransitionLight");
                    if (lightObj != null)
                        director.SetGenericBinding(track, lightObj.GetComponent<Animator>());
                    break;
            }
        }
    }

    /// <summary>
    /// 특정 이름으로 시작하는 트랙을 찾아서 오브젝트의 컴포넌트를 바인딩
    /// </summary>
    /// <param name="obj">바인딩할 게임오브젝트</param>
    /// <param name="trackNameStart">찾을 트랙 이름 시작 문자열</param>
    public void BindByTrackName(GameObject obj, string trackNameStart)
    {
        if (director.playableAsset is TimelineAsset timeline)
        {
            foreach (var track in timeline.GetOutputTracks())
            {
                if (track.name.StartsWith(trackNameStart))
                {
                    if (track is AnimationTrack)
                    {
                        var animator = obj.GetComponent<Animator>();
                        if (animator != null)
                        {
                            director.SetGenericBinding(track, animator);
                            Debug.Log($"AnimationTrack '{track.name}'에 바인딩 완료!");
                        }
                        else
                        {
                            Debug.LogWarning($"Animator가 {obj.name}에 없음!");
                        }
                    }
                    else if (track is SignalTrack)
                    {
                        var receiver = obj.GetComponent<SignalReceiver>();
                        if (receiver != null)
                        {
                            director.SetGenericBinding(track, receiver);
                            Debug.Log($"SignalTrack '{track.name}'에 바인딩 완료!");
                        }
                        else
                        {
                            Debug.LogWarning($"SignalReceiver가 {obj.name}에 없음!");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"지원되지 않는 트랙 타입: {track.GetType()}");
                    }
                }
            }
        }
    }

    public void PrintTimelineSubscribers()
    {
        if (OnTimelineChanged != null)
        {
            foreach (var d in OnTimelineChanged.GetInvocationList())
            {
                Debug.Log($"Subscriber method: {d.Method.Name}, Target: {d.Target}");
            }
        }
        else
        {
            Debug.Log("No subscribers");
        }
    }
}
