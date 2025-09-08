using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// DroppedItem 프리팹에 붙어있어야함
/// </summary>
public class DroppedItem : MonoBehaviour
{
    [HideInInspector] public ItemData ItemData;

    [SerializeField] new private Rigidbody2D rigidbody2D;

    public void Init(ItemData data, Vector3 dropPoint, Vector2 speedVector)
    {
        ItemData = data;
        gameObject.transform.position = dropPoint;
        rigidbody2D.linearVelocity = speedVector;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Pickup();
    }

    private void Pickup()
    {
        Player.Instance.inventory.AddItem(ItemData, 1);
        print(ItemData);

        // DroppedItemManager를 통해 적절한 풀에 반환
        DroppedItemManager.Instance.ReturnToPool(this);
    }
}
