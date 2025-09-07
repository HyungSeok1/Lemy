using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// DroppedItem 프리팹에 붙어있어야함
/// </summary>
public class DroppedItem : MonoBehaviour
{
    private IObjectPool<DroppedItem> pool;
    [HideInInspector] public ItemData ItemData;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] new private Rigidbody2D rigidbody2D;

    public void SetPool(IObjectPool<DroppedItem> poolRef)
    {
        pool = poolRef;
    }

    public void Init(ItemData data, Vector3 dropPoint, Vector2 speedVector)
    {
        ItemData = data;
        gameObject.transform.position = dropPoint;
        rigidbody2D.linearVelocity = speedVector;

        spriteRenderer.sprite = data.icon;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Pickup();
    }

    private void Pickup()
    {
        Player.Instance.inventory.AddItem(ItemData, 1);
        print(ItemData);
    
        pool.Release(this);
    }
}
