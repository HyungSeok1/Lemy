using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using System.Net;

public class DialogueManager : PersistentSingleton<DialogueManager>
{
    [Header("Main Ink File")]
    [SerializeField] private TextAsset inkJson;

    private Story story;
    private int currentChoiceIndex = -1;

    public bool dialoguePlaying = false;

    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";

    private InkExternalFunctions inkExternalFunctions;
    private InkDialogueVariables inkDialogueVariables;

    protected override void Awake()
    {
        base.Awake();

        story = new Story(inkJson.text);
        inkExternalFunctions = new InkExternalFunctions();
        inkExternalFunctions.Bind(story);
        inkDialogueVariables = new InkDialogueVariables(story);
    }


    private void OnDestroy()
    {
        inkExternalFunctions.Unbind(story);
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.dialogueEvents.onEnterDialogue += EnterDialogue;
        GameEventsManager.Instance.inputEvents.onSubmitPressed += SubmitPressed;
        GameEventsManager.Instance.dialogueEvents.onUpdateChoiceIndex += UpdateChoiceIndex;
        GameEventsManager.Instance.dialogueEvents.onUpdateInkDialogueVariable += UpdateInkDialogueVariable;
        GameEventsManager.Instance.questEvents.onQuestStateChange += QuestStateChange;

    }

    private void OnDisable()
    {
        if (GameEventsManager.Instance is null) return;
        GameEventsManager.Instance.dialogueEvents.onEnterDialogue -= EnterDialogue;
        GameEventsManager.Instance.inputEvents.onSubmitPressed -= SubmitPressed;
        GameEventsManager.Instance.dialogueEvents.onUpdateChoiceIndex -= UpdateChoiceIndex;
        GameEventsManager.Instance.dialogueEvents.onUpdateInkDialogueVariable -= UpdateInkDialogueVariable;
        GameEventsManager.Instance.questEvents.onQuestStateChange -= QuestStateChange;

    }

    private void QuestStateChange(Quest quest)
    {
        GameEventsManager.Instance.dialogueEvents.UpdateInkDialogueVariable(
            quest.info.id + "state",
            new StringValue(quest.state.ToString())
            );

        // 퀘스트 ID에 해당하는 변수명도 업데이트
        string questVariableName = quest.info.id + "state";
        if (story.variablesState.GlobalVariableExistsWithName(questVariableName))
        {
            story.variablesState[questVariableName] = quest.state.ToString();
        }
    }

    private void UpdateInkDialogueVariable(string name, Ink.Runtime.Object value)
    {
        inkDialogueVariables.UpdateVariableState(name, value);
    }

    private void UpdateChoiceIndex(int choiceIndex)
    {
        this.currentChoiceIndex = choiceIndex;
    }

    private void SubmitPressed(InputEventContext inputEventContext)
    {
        if (inputEventContext.Equals(InputEventContext.DEFAULT)) return; // DialogueManager가 먼저 등록됐을때 방어
        if (isEntering) return; // DialogTrigger가 등록됐을때 방어

        if (!DialoguePanel.Instance.canContinueToNextLine) // 글자들 출력 중..
        {
            DialoguePanel.Instance.skipRequested = true; // 스킵 요청하고 리턴.
            return;
        }

        // 선택지가 있고 선택된 선택지가 있을 때만 진행
        if (story.currentChoices.Count > 0)
        {
            if (currentChoiceIndex != -1)
            {
                ContinueOrExitStory();
            }
        }
        else
        {
            // 선택지가 없으면 일반적인 대화 진행
            ContinueOrExitStory();
        }
    }

    private bool isEntering;
    private void EnterDialogue(string knotName)
    {
        // 멈추기
        Player.Instance.movement.OnEnterDialogue();

        Player.Instance.playerInputController.EnableUIActionMap();

        if (dialoguePlaying) return;
        dialoguePlaying = true;
        isEntering = true;
        StartCoroutine(ResetEntering());

        GameEventsManager.Instance.dialogueEvents.DialogueStarted();
        GameEventsManager.Instance.inputEvents.ChangeInputEventContext(InputEventContext.DIALOGUE);

        story.ChoosePathString(knotName);

        // 모든 퀘스트 상태를 Ink 변수에 동기화
        SyncAllQuestStates();

        inkDialogueVariables.SyncVariablesAndStartListening(story);

        ContinueOrExitStory();
    }

    private IEnumerator ResetEntering()
    {
        yield return null;
        isEntering = false;
    }


    private void ContinueOrExitStory()
    {
        // 버튼눌러서 진행될때를 먼저 처리
        if (story.currentChoices.Count > 0 && currentChoiceIndex != -1)
        {
            story.ChooseChoiceIndex(currentChoiceIndex);
            currentChoiceIndex = -1; // reset choice index for next time
        }

        // 선택지 X. 그냥 Continue
        if (story.canContinue)
        {
            string dialogueLine = story.Continue();

            while (IsLineBlank(dialogueLine) && story.canContinue)
                dialogueLine = story.Continue();

            if (IsLineBlank(dialogueLine) && !story.canContinue)
                ExitDialogue();
            else
                GameEventsManager.Instance.dialogueEvents.DisplayDialogue(dialogueLine, story.currentChoices, story.currentTags);
        }
        else if (story.currentChoices.Count == 0) ExitDialogue();
    }

    /// <summary>
    /// 일단 지금은 컷씬에서만 씁니다. 나중에 리팩토링할수도.
    /// </summary>
    public void ContinueStory()
    {
        // 선택지 X. 그냥 Continue
        if (!story.canContinue)
        {
            Debug.LogError("진행될 스토리가 없음. 클립 다시 넣어줘야 할 듯.");
        }
        string dialogueLine = story.Continue();

        while (IsLineBlank(dialogueLine) && story.canContinue)
            dialogueLine = story.Continue();

        if (IsLineBlank(dialogueLine) && !story.canContinue)
            ExitDialogue();
        else
            GameEventsManager.Instance.dialogueEvents.DisplayDialogue(dialogueLine, story.currentChoices, story.currentTags);
    }

    public void ExitDialogue()
    {
        Player.Instance.playerInputController.EnablePlayerActionMap();
        dialoguePlaying = false;

        // inform other parts of our system that we've finished dialogue
        GameEventsManager.Instance.dialogueEvents.DialogueFinished();

        // let player move again
        // GameEventsManager.Instance.playerEvents.EnablePlayerMovement();

        // input event context
        GameEventsManager.Instance.inputEvents.ChangeInputEventContext(InputEventContext.DEFAULT);

        // stop listening for dialogue variables
        inkDialogueVariables.StopListening(story);

        // reset story state
        story.ResetState();
    }




    private void SyncAllQuestStates()
    {
        // QuestManager에서 모든 퀘스트 상태 가져와서 동기화
        QuestManager questManager = QuestManager.Instance;
        if (questManager != null)
        {
            var allQuests = questManager.GetAllQuests();
            foreach (var questPair in allQuests)
            {
                string questVariableName = questPair.Key + "state";
                if (story.variablesState.GlobalVariableExistsWithName(questVariableName))
                {
                    story.variablesState[questVariableName] = questPair.Value.state.ToString();
                }
            }
        }
    }

    private bool IsLineBlank(string dialogueLine)
    {
        return dialogueLine.Trim().Equals("") || dialogueLine.Trim().Equals("\n");
    }
}
