using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    // 연옥 브금을 재생하기 위한 임시 스크립트.
    void Start()
    {
        SoundManager.Instance.PlayBGM("yeonok_limit_bgm");
    }

    
}
