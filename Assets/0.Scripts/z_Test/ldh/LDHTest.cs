using UnityEngine;

public class LdhTest : MonoBehaviour
{
    [SerializeField] private float damage;
    public ItemData data1;
    public ItemData data2;
    public ItemData data3;


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            foreach (var mapData in MapDataManager.Instance.mapdataList)
            {
                foreach (var item in mapData.challengeZoneList)
                {
                    print(item.executionFlag);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            Player.Instance.inventory.AddItem(data1, 1);
            Player.Instance.inventory.AddItem(data2, 1);
            Player.Instance.inventory.AddItem(data3, 1);
        }


    }

}
