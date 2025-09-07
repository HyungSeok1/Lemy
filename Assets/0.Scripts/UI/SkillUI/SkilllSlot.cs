using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
///  새로 만든 스킬 UI
///  스킬 슬롯 코드
/// </summary>
public class SkillSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private GameObject dragClone; // 드래그용 복사본

    public ISkill skill; // ...스킬 데이터만 있으면 될줄 알았는데 아니어서 새로 만듦
    public Image skillImage;


    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        dragClone = GetComponent<GameObject>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 복사본 생성
        dragClone = new GameObject("DragClone");
        dragClone.transform.SetParent(canvas.transform, false);
        dragClone.transform.SetAsLastSibling();

        Image cloneImage = dragClone.AddComponent<Image>();
        cloneImage.sprite = skillImage.sprite;
        cloneImage.raycastTarget = false; // 복사본은 Raycast 받지 않음
        dragClone.transform.position = skillImage.transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragClone != null)
            dragClone.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragClone != null)
            Destroy(dragClone);
    }
}
