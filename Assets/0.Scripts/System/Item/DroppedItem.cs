using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// DroppedItem 프리팹에 붙어있어야함
/// </summary>
public class DroppedItem : MonoBehaviour
{
    public ItemData ItemData;

    public void Init(ItemData data, Vector3 dropPoint, Vector2 speedVector)
    {
        ItemData = data;
        gameObject.transform.position = dropPoint;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        Pickup();
    }

    private void Pickup()
    {
        if (ItemData.GetType() == typeof(MoneyItemData)) // 돈 아이템인 경우
        {
            MoneyItemData moneyData = ItemData as MoneyItemData;
            if (moneyData != null)
            {
                MoneyManager.Instance.AddMoney(moneyData.amount);
                Debug.Log("Money Picked up: " + moneyData.amount);
            }
        }
        else // 일반 아이템인 경우
        {
            //Player.Instance.inventory.AddItem(ItemData, 1);
            Sprite itemIcon = GetComponent<SpriteRenderer>().sprite;
            if (PopupUI.Instance != null)
            {
                PopupUI.Instance.ShowItemPopup(ItemData.itemName, itemIcon);
            }
            Debug.Log("Item Picked up: " + ItemData.itemName);
        }


        // DroppedItemManager를 통해 적절한 풀에 반환
        DroppedItemManager.Instance.ReturnToPool(this);
    }
}
