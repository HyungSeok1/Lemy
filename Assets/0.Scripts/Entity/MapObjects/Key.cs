using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

/// <summary>
/// 
/// 열쇠에 넣으면 되는 코드입니다.
/// keyData 스크립터블 오브젝트에 ID를 넣어주세요
/// 실행할거라면 컷씬도 실행하도록 할 수 있을겁니다. 아마도..
/// 
/// HasKey로 어디서든 
/// </summary>
public class Key : MonoBehaviour, ICutsceneObject
{
    [SerializeField] KeyData keyData;
    [SerializeField] GameObject keyEffect;

    private void Start()
    {
        if (HasKey(keyData.keyID)) gameObject.SetActive(false); // 이미 가지고있으면 삭제

        if (keyEffect == null)
        {
            keyEffect = transform.Find("KeyEffect").gameObject;
        }

        if (keyEffect != null)
        {
            var sr = keyEffect.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.DOKill(); // 혹시 모를 중복 애니메이션 제거
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
                sr.DOFade(0f, 1f) // 1초 동안 alpha 0까지
                  .SetLoops(-1, LoopType.Yoyo); // 무한 반복 (1 ↔ 0 ↔ 1)
            }
        }
    }

    /*
    private void OnEnable()
    {
        Debug.LogError("켜짐");
        // TimelineManager에 바인딩
        if (TimelineManager.Instance != null)
        {
            Debug.LogError("바인드");
            TimelineManager.Instance.BindByTrackName(gameObject, "Key");
        }
    }
    */
    private void OnEnable()
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (HasKey(keyData.keyID)) return; // 이미 가지고있으면 반응 X.

        if (collision.CompareTag("Player"))
        {
            if (keyData != null)
            {
                PlayerPrefs.SetInt($"Key_{keyData.keyID}", 1);
                PlayerPrefs.Save();

                Debug.Log($"Key {keyData.keyID} 획득!");
            }

            // 컷씬 실행
            TimelineManager.Instance.PlayTimeline(TimelineID.GetKey); // 컷씬에서 씬 변경도 추가할까 아니면 그냥 이 코드에 둔 채로 놔둘까....
            StartCoroutine(TestTransition());
        }
    }

    public void SetCameraToKey()
    {
        CameraSwitcher.ActiveCamera.Follow = transform;
    }


    private IEnumerator TestTransition()
    {
        yield return new WaitForSeconds(3f);
        SceneTransitionByKey();
    }

    public void SceneTransitionByKey()
    {
        SceneManager.LoadScene(keyData.ToMoveSceneName);
        Player.Instance.transform.position = Vector3.zero;
        CameraSwitcher.ActiveCamera.Follow = Player.Instance.transform;
    }

    // 키를 가졌는지 어디서든 체크 가능
    public static bool HasKey(int id)
    {
        return PlayerPrefs.GetInt($"Key_{id}", 0) == 1;
    }

    private void HandleTimelineChanged(PlayableDirector director, PlayableAsset asset)
    {
        // 교체될 때마다 자동 바인딩
        TimelineManager.Instance.BindByTrackName(gameObject, "Key");
    }

    public void BindCutsceneTrackReference()
    {
        throw new System.NotImplementedException();
    }
}

