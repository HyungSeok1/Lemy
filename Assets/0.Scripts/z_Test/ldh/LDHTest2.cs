using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LDHTest2 : MonoBehaviour
{
    public ItemData data1;
    public ItemData data2;

    public string json1;

    void Start()
    {
        json1 = JsonUtility.ToJson(data1);
        data2 = JsonUtility.FromJson<ItemData>(json1);

    }
}
