using UnityEngine;

public class LdhTest : MonoBehaviour
{
    [SerializeField] private float damage;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            SoundManager.Instance.PlayBGM("yeonok_limit_bgm");
        if (Input.GetKeyDown(KeyCode.S))
            SoundManager.Instance.PlayBGM("yeonok_limit_bgm");

    }

}
