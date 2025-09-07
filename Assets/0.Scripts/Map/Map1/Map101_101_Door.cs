using UnityEngine;
using UnityEngine.Playables;


/// <summary>
/// 맵 1 문 로직
/// 문 열림 관련 코드 수정해야함. 완전 난잡하게 짜여있음
/// 
/// </summary>
public class Map101_101_Door : MonoBehaviour
{
    bool opened = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    void Start()
    {
        if(PlayerPrefs.GetInt("Door1Opened", 0) == 1)
        {
            opened = true;
            Transform door = transform.Find("4_door");
            door.localPosition += new Vector3(0, 18 / transform.localScale.y, 0);

        }
        // 이벤트 구독
        TimelineManager.Instance.OnTimelineChanged += HandleTimelineChanged;
    }


    private void OnDisable()
    {
        // 이벤트 해제 (안 하면 메모리 릭)
        if (TimelineManager.Instance != null)
            TimelineManager.Instance.OnTimelineChanged -= HandleTimelineChanged;
    }


    private void OnTriggerStay2D(Collider2D otherCollider)
    {
        if(opened) return;

        if (!otherCollider.CompareTag("Player")) return;

        //if (Key.HasKey(101) && Key.HasKey(102) && Key.HasKey(103))
        {
            PlayerPrefs.SetInt($"Door1Opened", 1);
            PlayerPrefs.Save();

            opened = true;

            TimelineManager.Instance.GetComponent<PlayableDirector>().stopped += OnTimelineFinished;
            TimelineManager.Instance.BindByTrackName(gameObject, "Door");
            TimelineManager.Instance.PlayTimeline(TimelineID.Map1OpenDoor);
            CameraSwitcher.ActiveCamera.Follow = transform;
            Debug.Log("문 열림 재생");
            SoundManager.Instance.PlaySFX("doorOpen1", 1f);
        }
    }

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
}
