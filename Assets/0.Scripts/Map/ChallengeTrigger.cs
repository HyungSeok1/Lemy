using UnityEngine;

public class ChallengeTrigger : MonoBehaviour
{
    private enum ChallengeTriggerType { Zone, OnStart }

    [SerializeField] private string id;
    [SerializeField] private ChallengeZone challenge;

    [SerializeField] private ChallengeTriggerType triggerType;
    private bool flag; // 씬 들어왔을때 한번만 가능하도록 보장

    private void Awake()
    {
        flag = true;
    }

    private void Start()
    {
        if (!(triggerType == ChallengeTriggerType.OnStart)) return;
        TriggerChallenge();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!(triggerType == ChallengeTriggerType.Zone)) return;
        TriggerChallenge();
    }

    private void TriggerChallenge()
    {
        foreach (var challengeZone in MapDataManager.Instance.currentMapData.challengeZoneList)
            if (challengeZone.id == id && challengeZone.executionFlag && flag)
            {
                challenge.Challenge();
                flag = false; // 씬 안에선 한 번만
            }
    }
}
