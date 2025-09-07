using UnityEngine;
using UnityEngine.Timeline;

public class CutsceneTrigger : MonoBehaviour
{
    [SerializeField] private TimelineAsset cutscene; // 실행할 컷신만 보관

    public TimelineAsset Cutscene => cutscene; // 외부에서 읽기용 프로퍼티

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))  // 플레이어랑 부딪혔을 때만
            CutsceneManager.Instance.PlayCutscene(cutscene);
    }

}
