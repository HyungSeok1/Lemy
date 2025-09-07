using UnityEngine;
using UnityEngine.Pool;

public class DroppedItemManager : Singleton<DroppedItemManager>
{
    [SerializeField] private GameObject prefabReference;

    private IObjectPool<DroppedItem> pool;

    protected override void Awake()
    {
        base.Awake();

        pool = new ObjectPool<DroppedItem>(
            InitPool,
            OnGet,
            OnRelease,
            OnDestroyPooled,
            maxSize: 50
            );
    }
    // createFunc임. (새로 만들때 1회 호출)
    private DroppedItem InitPool()
    {
        var obj = Instantiate(prefabReference);
        var dropped = obj.GetComponent<DroppedItem>();

        // 풀로 되돌리기 위해 self-release 등록
        dropped.SetPool(pool);

        return dropped;
    }

    private void OnGet(DroppedItem item)
    {
        item.gameObject.SetActive(true);
    }

    private void OnRelease(DroppedItem item)
    {
        item.gameObject.SetActive(false);
    }

    private void OnDestroyPooled(DroppedItem item)
    {
        Destroy(item.gameObject);
    }

    public void Drop(ItemData data, Vector3 position, Vector2 speedVector)
    {
        var droppedItem = pool.Get();
        droppedItem.Init(data, position, speedVector);
    }
}
