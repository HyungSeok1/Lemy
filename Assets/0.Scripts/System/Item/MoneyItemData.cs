using System;
using UnityEngine;

[Serializable, CreateAssetMenu(fileName = "MoneyItemData", menuName = "Game/MoneyItemData")]
public class MoneyItemData : ItemData
{
    public int amount;
}