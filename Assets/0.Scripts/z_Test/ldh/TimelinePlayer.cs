using UnityEngine;
using UnityEngine.Playables;
public class TimelinePlayer : MonoBehaviour
{
    public PlayableDirector playableDirector;

    private void Update()
    {
        if (Input.anyKeyDown)
            playableDirector.gameObject.SetActive(true);
        playableDirector.Play();
    }

}
