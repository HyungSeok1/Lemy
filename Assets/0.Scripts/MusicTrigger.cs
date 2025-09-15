using UnityEngine;

public class MusicTrigger : MonoBehaviour
{

    [SerializeField] string bgmFileName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            SoundManager.Instance.PlayBGM(bgmFileName, 1f);
        }
    }
}
