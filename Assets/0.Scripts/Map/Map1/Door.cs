using UnityEngine;
using UnityEngine.Playables;


/// <summary>
/// 맵 1 문 로직
/// 
/// </summary>
public class Door : SaveableMapObject
{
    [SerializeField] private DoorConditionChecker checker;
    [SerializeField] private GameObject doorObject;

    bool opened = false;

    protected override void Start()
    {
        base.Start(); // 여기서 상태 적용 (activated OR not Activated)

        // 이벤트 구독
        TimelineManager.Instance.OnTimelineChanged += HandleTimelineChanged;
    }

    public override void ActivatedBehaviour()
    {
        base.ActivatedBehaviour();

        doorObject.transform.localPosition += new Vector3(8.97f, 18 / transform.localScale.y, 0);
        opened = true;
    }

    public override void NotActivatedBehaviour()
    {
        base.NotActivatedBehaviour();
    }


    private void OnTriggerStay2D(Collider2D otherCollider)
    {
        if (opened) return;

        if (!otherCollider.CompareTag("Player")) return;

        if (checker.canOpen)
        {
            checker.ConsumeKeys(); // 키를 인벤에서 삭제

            foreach (var mapObjectEntry in MapDataManager.Instance.currentMapData.saveableMapObjectList)
            {
                if (mapObjectEntry.id != id) continue;
                mapObjectEntry.isActivated = true;
            }
            opened = true;

            TimelineManager.Instance.GetComponent<PlayableDirector>().stopped += OnTimelineFinished;
            TimelineManager.Instance.BindByTrackName(gameObject, "Door");
            TimelineManager.Instance.PlayTimeline(TimelineID.Map1OpenDoor);
            CameraSwitcher.ActiveCamera.Follow = transform;
            SoundManager.Instance.PlaySFX("doorOpen1", 1f);
        }
    }

    #region 타임라인 관련
    private void OnTimelineFinished(PlayableDirector director)
    {
        TimelineManager.Instance.GetComponent<PlayableDirector>().stopped -= OnTimelineFinished;

        //카메라 돌려주기
        CameraSwitcher.ActiveCamera.Follow = Player.Instance.transform;
    }

    private void HandleTimelineChanged(PlayableDirector director, PlayableAsset asset)
    {
        // 교체될 때마다 자동 바인딩
        TimelineManager.Instance.BindByTrackName(gameObject, "Door");
    }

    private void OnDisable()
    {
        // 이벤트 해제 (안 하면 메모리 릭)
        if (TimelineManager.Instance != null)
            TimelineManager.Instance.OnTimelineChanged -= HandleTimelineChanged;
    } 
    #endregion
}
