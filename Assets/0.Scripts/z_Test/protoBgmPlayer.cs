using UnityEngine;

public class protoBgmPlayer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SoundManager.Instance.PlayBGM("yeonok_bgm");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
