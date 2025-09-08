using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Game/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    private Dictionary<string, ItemData> idItemMap;
    [Header("")]
    [SerializeField] private List<ItemData> db = new();

    private void OnEnable()
    {
        Load();
    }

    private void Load()
    {
        // id가 비거나 중복인 항목은 첫 항목만 채택
        idItemMap = db
            .Where(i => i != null && !string.IsNullOrWhiteSpace(i.itemName))
            .GroupBy(i => i.itemName)
            .ToDictionary(g => g.Key, g => g.First());
    }

    /// 세이브용 ID
    public string GetId(ItemData item) => item ? item.itemName : null;

    /// 로드시 ID → ItemData 복구
    public ItemData GetById(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return null;
        return idItemMap.TryGetValue(id, out var it) ? it : null;
    }
}
