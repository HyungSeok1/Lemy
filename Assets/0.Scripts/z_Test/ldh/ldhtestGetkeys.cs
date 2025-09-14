using UnityEngine;

public class ldhtestGetkeys : MonoBehaviour
{
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private ItemData[] keys;              // key1_1_2Data, key1_1_3Data, key1_1_4Data

    void Awake()
    {
        foreach (var key in keys)
            inventory.AddItem(key, 1);
    }
}
