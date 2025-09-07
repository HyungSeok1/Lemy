using UnityEngine.Timeline;
using UnityEngine;
using UnityEngine.Playables;

[TrackColor(1f, 0.2f, 0.8f)] // 타임라인 트랙 색상
[TrackClipType(typeof(ActorControlAsset))]
public class ActorControlTrack : TrackAsset
{
    public ExposedReference<Animator> animator;
    public ExposedReference<Transform> transform;
    protected override void OnCreateClip(TimelineClip clip)
    {
        base.OnCreateClip(clip);
        // 새 클립 생성 시 트랙의 animator를 클립으로 복사
        if (clip.asset is ActorControlAsset asset)
        {
            asset.exposedAnimator = animator;
            asset.exposedTransform = transform;
        }
    }

#if UNITY_EDITOR
    public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
    {
        var bound = director.GetGenericBinding(this) as Transform;
        if (bound != null)
        {
            // 스크럽/레코딩 시 Transform 위치를 타임라인이 추적하도록 등록
            driver.AddFromName<Transform>(bound.gameObject, "m_LocalPosition.x");
            driver.AddFromName<Transform>(bound.gameObject, "m_LocalPosition.y");
            driver.AddFromName<Transform>(bound.gameObject, "m_LocalPosition.z");
        }
        base.GatherProperties(director, driver);
    }
#endif

    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<ActorControlMixerBehaviour>.Create(graph, inputCount);
    }
}