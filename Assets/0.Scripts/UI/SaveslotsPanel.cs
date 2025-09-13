using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 메인화면에서 StartGame버튼 누를시 작동하는 코드
/// </summary>
public class SaveslotsPanel : MonoBehaviour
{
    public GameObject SlotParent;
    public SaveloadSlotScript slot1Script;
    public SaveloadSlotScript slot2Script;
    public SaveloadSlotScript slot3Script;


    public void ActiveSaveloadSlots()
    {
        SlotParent.SetActive(true);
        slot1Script.RenewText(1);
        slot2Script.RenewText(2);
        slot3Script.RenewText(3);
    }



}
