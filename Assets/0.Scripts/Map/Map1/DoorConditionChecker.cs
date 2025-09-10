using DG.Tweening;
using System.Linq;
using UnityEngine;

public class DoorConditionChecker : MonoBehaviour
{
    public bool canOpen = false;
    private bool[] lamps = new bool[3];                                  // 각 문마다 lamp 상태 저장
    [SerializeField] private ItemData[] keys;              // key1_1_2Data, key1_1_3Data, key1_1_4Data
    [SerializeField] private GameObject[] doors;           // doorOpened1, doorOpened2, doorOpened3

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateDoors();
    }

    private void UpdateDoors()
    {
        for (int i = 0; i < keys.Length; i++)
        {
            bool hasItem = Player.Instance.inventory.HasItem(keys[i]);
            SpriteRenderer sr = doors[i].GetComponent<SpriteRenderer>();
            sr.color = hasItem ? Color.white : Color.black;
            lamps[i] = hasItem;
        }

        if (lamps.All(l => l))
            canOpen = true;
    }

    public void ConsumeKeys()
    {
        foreach (var key in keys)
        {
            Player.Instance.inventory.TryConsume(key);
        }
    }
#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U)) // 갱신
            UpdateDoors();
    }
#endif
}
