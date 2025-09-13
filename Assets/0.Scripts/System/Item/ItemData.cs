using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Game/ItemData")]
public class ItemData : ScriptableObject
{
    public Sprite icon;
    public string itemName; // ItemDatabase에서의 매핑에 쓰임
    public bool stackable;
}


/// <summary>
/// 저장시엔 이걸 사용함.
/// </summary>
[Serializable]
public class InventorySlotDTO
{
    public string itemId;
    public int count;
}