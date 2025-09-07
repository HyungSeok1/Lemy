using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CoverImage : MonoBehaviour
{
    private void Start()
    {
        // 이벤트 구독
        TimelineManager.Instance.OnTimelineChanged += HandleTimelineChanged;
    }
    
    private void OnDisable()
    {
        // 이벤트 해제 (안 하면 메모리 릭)
        if (TimelineManager.Instance != null)
            TimelineManager.Instance.OnTimelineChanged -= HandleTimelineChanged;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void HandleTimelineChanged(PlayableDirector director, PlayableAsset asset)
    {
        // 교체될 때마다 자동 바인딩
        TimelineManager.Instance.BindByTrackName(gameObject, "CoverImage");
    }
}
