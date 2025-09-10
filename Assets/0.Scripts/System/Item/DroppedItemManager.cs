using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;
using System;

[Serializable]
public class DroppedItemPrefabMapping
{
    public ItemData itemData;
    public GameObject prefab;
}

public class DroppedItemManager : Singleton<DroppedItemManager>
{
    [SerializeField] private GameObject defaultPrefabReference;
    [SerializeField] private List<DroppedItemPrefabMapping> prefabMappings = new List<DroppedItemPrefabMapping>();

    [Space(10)]
    [Header("Drop settings")]
    [SerializeField] private float RandomDropRadius = 1.5f;

    private Dictionary<ItemData, IObjectPool<DroppedItem>> pools = new Dictionary<ItemData, IObjectPool<DroppedItem>>();
    private Dictionary<ItemData, GameObject> prefabDict = new Dictionary<ItemData, GameObject>();

    protected override void Awake()
    {
        base.Awake();

        // 프리팹 매핑 딕셔너리 초기화
        foreach (var mapping in prefabMappings)
        {
            if (mapping.itemData != null && mapping.prefab != null)
            {
                prefabDict[mapping.itemData] = mapping.prefab;
            }
        }
    }

    // 특정 ItemData에 대한 풀을 가져오거나 생성
    private IObjectPool<DroppedItem> GetOrCreatePool(ItemData itemData)
    {
        if (!pools.ContainsKey(itemData))
        {
            pools[itemData] = new ObjectPool<DroppedItem>(
                () => InitPool(itemData),
                OnGet,
                OnRelease,
                OnDestroyPooled,
                maxSize: 50
            );
        }
        return pools[itemData];
    }

    // createFunc임. (새로 만들때 1회 호출)
    private DroppedItem InitPool(ItemData itemData)
    {
        // 해당 아이템에 맞는 프리팹을 찾거나 기본 프리팹 사용
        GameObject prefabToUse = prefabDict.ContainsKey(itemData) ? prefabDict[itemData] : defaultPrefabReference;

        if (prefabToUse == null)
        {
            Debug.LogError($"[DroppedItemManager] 프리팹이 없습니다. ItemData: {itemData?.itemName ?? "null"}");
            return null;
        }

        var obj = Instantiate(prefabToUse);
        var dropped = obj.GetComponent<DroppedItem>();

        if (dropped == null)
        {
            Debug.LogError($"[DroppedItemManager] 프리팹에 DroppedItem 컴포넌트가 없습니다. Prefab: {prefabToUse.name}");
            Destroy(obj);
            return null;
        }

        return dropped;
    }

    private void OnGet(DroppedItem item)
    {
        if (item == null)
        {
            Debug.LogError("[DroppedItemManager] OnGet에서 item이 null입니다.");
            return;
        }
        item.gameObject.SetActive(true);
    }

    private void OnRelease(DroppedItem item)
    {
        if (item == null)
        {
            Debug.LogError("[DroppedItemManager] OnRelease에서 item이 null입니다.");
            return;
        }
        item.gameObject.SetActive(false);
    }

    private void OnDestroyPooled(DroppedItem item)
    {
        if (item != null)
            Destroy(item.gameObject);
    }

    public void Drop(ItemData data, Vector3 position, Vector2 speedVector)
    {
        if (data == null)
        {
            Debug.LogError("[DroppedItemManager] Drop에서 ItemData가 null입니다.");
            return;
        }

        var pool = GetOrCreatePool(data);
        var droppedItem = pool.Get();

        if (droppedItem == null)
        {
            Debug.LogError($"[DroppedItemManager] 풀에서 DroppedItem을 가져올 수 없습니다. ItemData: {data.itemName}");
            return;
        }

        droppedItem.Init(data, position, speedVector);
    }

    public void Drop(ItemData data, int count, Vector3 position, Vector2 speedVector)
    {
        if (data == null)
        {
            Debug.LogError("[DroppedItemManager] Drop에서 ItemData가 null입니다.");
            return;
        }

        if (count == 1)
        {
            Drop(data, position, speedVector);
            return;
        }

        var pool = GetOrCreatePool(data);

        for (int i = 0; i < count; i++)
        {
            var droppedItem = pool.Get();

            if (droppedItem == null)
            {
                Debug.LogError($"[DroppedItemManager] 풀에서 DroppedItem을 가져올 수 없습니다. ItemData: {data.itemName}");
                continue;
            }

            // 약간의 랜덤 오프셋과 속도 벡터를 추가하여 아이템이 흩어지도록 함
            Vector3 randomOffset = new Vector3(UnityEngine.Random.Range(-RandomDropRadius, RandomDropRadius), UnityEngine.Random.Range(-RandomDropRadius, RandomDropRadius), 0);
            Vector2 randomSpeed = speedVector + new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));

            droppedItem.Init(data, position + randomOffset, randomSpeed);
        }
    }



    public void ReturnToPool(DroppedItem droppedItem)
    {
        if (droppedItem?.ItemData == null) return;

        var pool = GetOrCreatePool(droppedItem.ItemData);
        pool.Release(droppedItem);
    }
}
