using UnityEngine;
using UnityEngine.UI;

public class SaveSelectPanel : PersistentSingleton<SaveSelectPanel>
{
    public GameObject SlotParent;
    public SaveloadSlotScript slot1Script;
    public SaveloadSlotScript slot2Script;
    public SaveloadSlotScript slot3Script;


    public void OnSaveButtonPressed(int slot)
    {
        SaveLoadManager.Instance.SaveGame(slot);
        SlotParent.SetActive(false);
    }

    public void OnLoadButtonPressed(int slot)
    {
        // TODO: 로드 구현 - 지금은 바로 LoadGame해버림. 지연이 필요할듯
        SaveLoadManager.Instance.LoadGame(slot);
        SlotParent.SetActive(false);

    }


    public void ActiveSaveloadSlots()
    {
        SlotParent.SetActive(true);
        slot1Script.RenewText(1);
        slot2Script.RenewText(2);
        slot3Script.RenewText(3);
    }



}
