using UnityEngine;

public class SaveableMapObject : MonoBehaviour
{
    public string id;

    protected virtual void Start()
    {
        Activate();
    }

    /// <summary>
    /// Start에서 실행
    /// </summary>
    public virtual void Activate()
    {
        foreach (var mapObjectEntry in MapDataManager.Instance.currentMapData.saveableMapObjectList)
        {
            if (mapObjectEntry.id != id) continue;

            if (mapObjectEntry.isActivated) ActivatedBehaviour();
            else NotActivatedBehaviour();
        }
    }

    /// <summary>
    /// 세이브 데이터에서 isActivated가 true일때 (ex. 예전에 이미 Door 열었을때)
    /// </summary>
    public virtual void ActivatedBehaviour()
    {

    }

    /// <summary>
    /// 세이브 데이터에서 isActivated가 false일때 (ex. 예전에 Door를 연 적이 없을 때)
    /// </summary>
    public virtual void NotActivatedBehaviour()
    {

    }

}
