using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DialogueChoiceButton : MonoBehaviour, ISelectHandler
{
    [Header("Components")]
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI choiceText;
    [SerializeField] private GameObject selectUI; // 선택 상태를 표시할 UI 오브젝트

    private int choiceIndex = -1;

    private void Start()
    {
        // 초기에는 selectUI를 비활성화
        if (selectUI != null)
        {
            selectUI.SetActive(false);
        }
    }

    public void SetChoiceText(string choiceTextString)
    {
        choiceText.text = choiceTextString;
    }

    public void SetChoiceIndex(int choiceIndex)
    {
        this.choiceIndex = choiceIndex;
    }

    public void SelectButton()
    {
        Debug.Log("Button selected: " + choiceIndex);
        GameEventsManager.Instance.dialogueEvents.UpdateChoiceIndex(choiceIndex);

        button.Select();
        ShowSelectUI();

        GameEventsManager.Instance.inputEvents.SubmitPressed(); // 추가됨. (버튼누르면 즉시 Continue)
    }

    public void HideSelectUI()
    {
        selectUI.SetActive(false);
    }

    public void ShowSelectUI()
    {
        selectUI.SetActive(true);
    }

    public void OnSelect(BaseEventData eventData)
    {
        GameEventsManager.Instance.dialogueEvents.UpdateChoiceIndex(choiceIndex);
        ShowSelectUI();
    }

    private void OnMouseEnter()
    {
        if (Input.GetMouseButton(0))
        {

        }
    }

    private void OnMouseLeave()
    {

    }
}