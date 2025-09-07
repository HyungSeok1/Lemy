using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SaveloadSlotScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject slotButtonParent;

    [SerializeField] private TMP_Text fileNumText;
    [SerializeField] private TMP_Text chapterAndMapText;
    [SerializeField] private TMP_Text elapsedTimeText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        slotButtonParent.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        slotButtonParent.SetActive(false);
    }

    public void RenewText(int slot)
    {
        var meta = SaveLoadManager.Instance.PeekMeta(slot);

        fileNumText.text = $"File {meta.slot}";
        chapterAndMapText.text = $"Chapter {meta.chapter} - Map {meta.map}";
        //TODO: 플레이타임 계산 & 입력
    }
}
