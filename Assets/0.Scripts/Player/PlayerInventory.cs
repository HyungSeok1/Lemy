using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class InventorySlot
{
    public ItemData data;
    public int count;
}

public class PlayerInventory : MonoBehaviour, ISaveable<InventoryData>
{
    [SerializeField] private ItemDatabase db;
    [SerializeField] private List<InventorySlot> slots = new();


    public event Action OnChanged;

    /// <summary>
    /// ItemData로 아이템 개수 조회
    /// </summary>
    public int GetItemCount(ItemData data)
    {
        if (data == null) return 0;

        int sum = 0;
        foreach (var s in slots)
            if (s.data.itemName == data.itemName) sum += Mathf.Max(0, s.count);

        return sum;
    }

    /// <summary>
    /// 아이템 ID로 아이템 개수 조회
    /// </summary>
    public int GetItemCount(string itemID)
    {
        ItemData data = db.GetById(itemID);
        return GetItemCount(data);
    }

    /// <summary>
    /// ItemData로 아이템 보유 여부 조회 (개수 포함)
    /// </summary>
    public bool HasItem(ItemData data, int count = 1)
    {
        return GetItemCount(data) >= count;
    }

    /// <summary>
    /// 아이템 ID로 아이템 보유 여부 조회 (개수 포함)
    /// </summary>
    public bool HasItem(string itemID, int count = 1)
    {
        ItemData data = db.GetById(itemID);
        return HasItem(data, count);
    }

    /// <summary>
    /// ItemData로 아이템 소모 (보유 개수 부족 시 false)
    /// </summary>
    /// <param name="data"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public bool TryConsume(ItemData data, int count = 1)
    {
        if (!HasItem(data, count)) return false;
        RemoveItem(data, count);
        return true;
    }

    /// <summary>
    /// 아이템 ID로 아이템 소모 (보유 개수 부족 시 false)
    /// </summary>
    public bool TryConsume(string itemID, int count = 1)
    {
        ItemData data = db.GetById(itemID);
        return TryConsume(data, count);
    }

    public void AddItem(ItemData data, int count)
    {
        if (data == null || count <= 0) return;

        if (data.stackable)
        {
            var slot = slots.Find(s => s.data == data);
            if (slot != null) slot.count += count;
            else slots.Add(new InventorySlot { data = data, count = count });
        }
        else
        {
            for (int i = 0; i < count; i++)
                slots.Add(new InventorySlot { data = data, count = 1 });
        }

        Debug.Log($"Item Added: {data.itemName} x{count}");
        OnChanged?.Invoke();
    }

    public void AddItem(string itemID, int count)
    {
        ItemData data = db.GetById(itemID);
        AddItem(data, count);
    }

    public void RemoveItem(ItemData data, int count)
    {
        if (data == null || count <= 0) return;
        if (!HasItem(data, count)) return;

        if (data.stackable)
        {
            var slot = slots.Find(s => s.data == data);
            if (slot == null) return;
            slot.count -= count;
            if (slot.count <= 0) slots.Remove(slot);
        }
        else
        {
            for (int i = slots.Count - 1; i >= 0 && count > 0; i--)
            {
                if (slots[i].data == data)
                {
                    slots.RemoveAt(i);
                    count--;
                }
            }
        }

        Debug.Log($"Item Removed: {data.itemName} x{count}");
        OnChanged?.Invoke();
    }

    public void RemoveItem(string itemID, int count)
    {
        ItemData data = db.GetById(itemID);
        RemoveItem(data, count);
    }

    public void Save(ref InventoryData data)
    {
        // 참조 공유 X, DTO로 투영
        data.slots = new List<InventorySlotDTO>(slots.Count);
        foreach (var s in slots)
        {
            if (s?.data == null || s.count <= 0) continue;
            string id = db.GetId(s.data);
            if (id == null) { Debug.LogWarning($"아이디가 db에 없음 {s.data}"); continue; }

            data.slots.Add(new InventorySlotDTO
            {
                itemId = id,
                count = s.count
            });
        }
    }

    public void Load(InventoryData data)
    {
        slots = new List<InventorySlot>(data?.slots?.Count ?? 0);

        if (data?.slots != null)
        {
            foreach (var dto in data.slots)
            {
                var item = db.GetById(dto.itemId);
                if (item == null) { Debug.LogWarning($"아이템이 db에 없음: {dto.itemId}"); continue; }

                slots.Add(new InventorySlot
                {
                    data = item,
                    count = dto.count
                });
            }
        }

        OnChanged?.Invoke();
    }
}
