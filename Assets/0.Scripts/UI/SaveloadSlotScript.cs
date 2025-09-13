using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XInput;
using System.IO;

/// <summary>
/// 각각의 세이브 슬롯
/// </summary>
public class SaveloadSlotScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private int slot;
    private bool isEmpty;

    [SerializeField] private TMP_Text emptyFileText;
    [SerializeField] private TMP_Text fileNumText;
    [SerializeField] private TMP_Text chapterAndMapText;
    [SerializeField] private TMP_Text elapsedTimeText;

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

    public void RenewText(int slot)
    {
        this.slot = slot;

        string path = SaveLoadManager.Instance.GetMetaPathForSlot(slot);
        string json = File.ReadAllText(path);   
        var metaData = JsonUtility.FromJson<SaveMeta>(json);

        // 임시 하드코딩
        if (metaData.isEmpty)
        {
            // 비었을때 true
            emptyFileText.gameObject.SetActive(true);

            fileNumText.gameObject.SetActive(!metaData.isEmpty);
            chapterAndMapText.gameObject.SetActive(!metaData.isEmpty);
            elapsedTimeText.gameObject.SetActive(!metaData.isEmpty);
        }
        else
        { 
            //차있을때  false

            emptyFileText.gameObject.SetActive(false);

            fileNumText.gameObject.SetActive(!metaData.isEmpty);
            chapterAndMapText.gameObject.SetActive(!metaData.isEmpty);
            elapsedTimeText.gameObject.SetActive(!metaData.isEmpty);

            fileNumText.text = $"File {slot}";
            chapterAndMapText.text = $"{metaData.stateData.chapter}-{metaData.stateData.map}-{metaData.stateData.number}";
            elapsedTimeText.text = $"{metaData.playtimeData.totalPlaytime}";
        }

        isEmpty = metaData.isEmpty;
    }

    /// <summary>
    /// 패널 그 자체(버튼 역할) 클릭시 작동
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isEmpty) SaveLoadManager.Instance.LoadNewGame(slot);
        else SaveLoadManager.Instance.LoadGame(slot);
    }
}
