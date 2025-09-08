using UnityEngine;

public class ChallengeTrigger : MonoBehaviour
{
    [SerializeField] private string id;
    [SerializeField] private ChallengeZone zone;


    private bool flag;

    private void Awake()
    {
        flag = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (var challengeZone in MapDataManager.Instance.currentMapData.challengeZoneList)
            if (challengeZone.id == id && challengeZone.executionFlag && flag)
            {
                zone.Challenge();
                flag = false; // 씬 안에선 한 번만
            }

    }
}
