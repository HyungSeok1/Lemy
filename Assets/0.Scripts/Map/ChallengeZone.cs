using UnityEngine;

public class ChallengeZone : MonoBehaviour
{
    public string id;

    /// <summary>
    /// 이거 호출해서 작동. - ontriggetEnter, 참조 받아오기 등으로 다른곳에서 호출. (직접 스스로 호출 XXXXXX)
    /// </summary>
    public virtual void Challenge()
    {

    }

    /// <summary>
    /// 깼을때만 flag false로
    /// </summary>
    public virtual void BeatChallenge()
    {
        bool existFlag = false;
        foreach (var challengeZone in MapDataManager.Instance.currentMapData.challengeZoneList)
            if (challengeZone.id == id)
            {
                existFlag = true;
                challengeZone.executionFlag = false;
            }

        if (!existFlag)
            Debug.LogError("id 존재 안함");
    }
}
