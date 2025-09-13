using UnityEngine;


/// <summary>
/// Scene 1_1_4 
/// </summary>
public class ObstacleChallenge : ChallengeZone
{
    [SerializeField] private GameObject keyPrefab;

    public override void Challenge()
    {
        base.Challenge();
        SpawnKey();
    }

    public override void BeatChallenge()
    {
        base.BeatChallenge();
    }

    private void SpawnKey()
    {
        GameObject key = Instantiate(keyPrefab, transform.position, Quaternion.identity, transform);
        key.SetActive(true);
        key.GetComponent<Key>().OnGetKey += BeatChallenge;
    }
}
