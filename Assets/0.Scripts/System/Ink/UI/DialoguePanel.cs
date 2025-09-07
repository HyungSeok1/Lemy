using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.UI;
using System.Linq;

public class DialoguePanel : PersistentSingleton<DialoguePanel>
{
    [Header("Components")]
    [SerializeField] private GameObject contentParent;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private DialogueChoiceButton[] choiceButtons;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Animator portraitAnimator;
    [SerializeField] private GameObject continueIcon;
    [SerializeField] private float typingSpeed = 0.04f;

    public AutoMessagePanel autoMessagePanel;

    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";

    private Coroutine displayLineCoroutine;
    public bool canContinueToNextLine;


    protected override void Awake()
    {
        base.Awake();
        contentParent.SetActive(false);
        ResetPanel();
    }

    #region 대화창( Dialog ) 
    private void OnEnable()
    {
        GameEventsManager.Instance.dialogueEvents.onDialogueStarted += DialogueStarted;
        GameEventsManager.Instance.dialogueEvents.onDialogueFinished += DialogueFinished;
        GameEventsManager.Instance.dialogueEvents.onDisplayDialogue += DisplayDialogue;
        GameEventsManager.Instance.dialogueEvents.onUpdateChoiceIndex += OnChoiceIndexUpdated;
    }

    private void OnDisable()
    {
        if (GameEventsManager.Instance is null) return;
        GameEventsManager.Instance.dialogueEvents.onDialogueStarted -= DialogueStarted;
        GameEventsManager.Instance.dialogueEvents.onDialogueFinished -= DialogueFinished;
        GameEventsManager.Instance.dialogueEvents.onDisplayDialogue -= DisplayDialogue;
        GameEventsManager.Instance.dialogueEvents.onUpdateChoiceIndex -= OnChoiceIndexUpdated;
    }

    private void OnChoiceIndexUpdated(int selectedChoiceIndex)
    {
        // 모든 버튼의 selectUI를 꺼줌
        foreach (DialogueChoiceButton choiceButton in choiceButtons)
        {
            if (choiceButton.gameObject.activeInHierarchy)
            {
                choiceButton.HideSelectUI();
            }
        }

        // 선택된 버튼의 selectUI만 켜줌
        if (selectedChoiceIndex >= 0 && selectedChoiceIndex < choiceButtons.Length)
        {
            if (choiceButtons[selectedChoiceIndex].gameObject.activeInHierarchy)
            {
                choiceButtons[selectedChoiceIndex].ShowSelectUI();
            }
        }
    }

    private void DialogueStarted()
    {
        contentParent.SetActive(true);
    }

    private void DialogueFinished()
    {
        contentParent.SetActive(false);

        // reset anything for next time
        ResetPanel();
    }

    private void DisplayDialogue(string dialogueLine, List<Choice> dialogueChoices, List<string> currentTags)
    {
        HandleTags(currentTags);

        if (displayLineCoroutine != null)
            StopCoroutine(displayLineCoroutine);
        displayLineCoroutine = StartCoroutine(DisplayLine(dialogueLine, dialogueChoices));
    }
    public bool skipRequested = false;
    private IEnumerator DisplayLine(string line, List<Choice> dialogueChoices)
    {
        // 아이콘, 버튼 숨기기
        continueIcon.SetActive(false);
        HideChoices();

        // 빈 텍스트 준비
        dialogueText.text = "";
        canContinueToNextLine = false;
        bool isAddingRichTextTag = false;

        yield return null;

        // display each letter one at a time
        foreach (char letter in line.ToCharArray())
        {
            // 다시 상호작용 -> 스킵
            if (skipRequested)
            {
                dialogueText.text = line;
                skipRequested = false; // 플래그 소비
                break;
            }

            // 색깔 텍스트 처리
            if (letter == '<' || isAddingRichTextTag)
            {
                isAddingRichTextTag = true;
                dialogueText.text += letter;
                if (letter == '>')
                {
                    isAddingRichTextTag = false;
                }
            }
            // 속도
            else
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        // 다 끝나고 아이콘 & 버튼 켜기
        continueIcon.SetActive(true);

        // defensive check - if there are more choices coming in than we can support, log an error
        if (dialogueChoices.Count > choiceButtons.Length)
        {
            Debug.LogError("More dialogue choices ("
                + dialogueChoices.Count + ") came through than are supported ("
                + choiceButtons.Length + ").");
        }

        /////// ==>>>  <버튼로직 시작>: 선택지가 있을 때는 현재 선택된 선택지 인덱스를 초기화 (여기서 버튼 꺼줌)
        if (dialogueChoices.Count > 0)
        {
            GameEventsManager.Instance.dialogueEvents.UpdateChoiceIndex(-1);
        }

        // 일단 버튼 다 끄기
        foreach (DialogueChoiceButton choiceButton in choiceButtons)
        {
            choiceButton.gameObject.SetActive(false);
            choiceButton.HideSelectUI(); // selectUI도 비활성화
        }

        // 초이스 버튼 켜기 & 값 할당
        int choiceButtonIndex = 0;
        for (int inkChoiceIndex = 0; (inkChoiceIndex < dialogueChoices.Count && inkChoiceIndex < choiceButtons.Count()); inkChoiceIndex++)
        {
            Choice dialogueChoice = dialogueChoices[inkChoiceIndex];
            DialogueChoiceButton choiceButton = choiceButtons[choiceButtonIndex];

            choiceButton.gameObject.SetActive(true);
            choiceButton.SetChoiceText(dialogueChoice.text);
            choiceButton.SetChoiceIndex(inkChoiceIndex);
            choiceButtonIndex++;
        }

        canContinueToNextLine = true;
    }

    private void HideChoices()
    {
        foreach (var choiceButton in choiceButtons)
        {
            choiceButton.gameObject.SetActive(false);
            choiceButton.HideSelectUI();
        }
    }

    private void HandleTags(List<string> currentTags)
    {
        foreach (string tag in currentTags)
        {
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)
            {
                Debug.LogError("Tag couldn't be appropriately parsed: " + tag);
            }
            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            switch (tagKey)
            {
                case SPEAKER_TAG:
                    nameText.text = tagValue;
                    break;
                case PORTRAIT_TAG:
                    portraitAnimator.Play(tagValue);
                    break;
                default:
                    Debug.LogWarning("Tag came in but is not currently being handled" + tag);
                    break;
            }
        }
    }

    private void ResetPanel()
    {
        dialogueText.text = "";
    }
    #endregion
}
