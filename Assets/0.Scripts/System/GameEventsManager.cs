using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
/// <summary>
/// Dialog, Quest를 담당하는
/// 
/// Ink 플러그인의 이벤트들을 처리하는 클래스입니다.

/// </summary>
public class GameEventsManager : PersistentSingleton<GameEventsManager>
{
    public InputEvents inputEvents;
    public QuestEvents questEvents;
    public DialogueEvents dialogueEvents;
    public BattleEvents battleEvents;
    public MiscEvents miscEvents;

    protected override void Awake()
    {
        base.Awake();

        inputEvents = new InputEvents();
        questEvents = new QuestEvents();
        dialogueEvents = new DialogueEvents();
        battleEvents = new BattleEvents();
        miscEvents = new MiscEvents();
    }
}

public enum InputEventContext
{
    DEFAULT,
    DIALOGUE
}

public class InputEvents
{
    public InputEventContext inputEventContext { get; private set; }
    public void ChangeInputEventContext(InputEventContext newContext)
    {
        this.inputEventContext = newContext;
    }

    public event Action<InputEventContext> onSubmitPressed;
    public void SubmitPressed()
    {

        if (onSubmitPressed != null)
        {
            onSubmitPressed(this.inputEventContext); // Action Invoke
        }
    }

    public event Action<InputEventContext> onPointAttack;
    public event Action<InputEventContext> onDragAttack;
    public event Action<InputEventContext> onDragButNotAttack;
    public event Action<InputEventContext> onFinalAttack;

    public void PlayerAttack(int attackType)
    {
        // 0: point attack
        // 1: drag_and_drop attack
        // 2: drag_and_drop but not attack due to the angle
        // 3: final attack
        switch (attackType)
        {
            case 0:
                if (onPointAttack != null)
                {
                    onPointAttack(this.inputEventContext);
                }
                break;
            case 1:
                if (onDragAttack != null)
                {
                    onDragAttack(this.inputEventContext);
                }
                break;
            case 2:
                if (onDragButNotAttack != null)
                {
                    onDragButNotAttack(this.inputEventContext);
                }
                break;
            case 3:
                if (onFinalAttack != null)
                {
                    onFinalAttack(this.inputEventContext);
                }
                break;
            default:

                break;

        }
    }


}

public class QuestEvents
{
    public event Action<string> onStartQuest;
    public void StartQuest(string id)
    {

        if (onStartQuest != null)
        {
            onStartQuest(id);
        }
    }

    public event Action<string, QuestState> onAdvanceQuest;
    public void AdvanceQuest(string id, QuestState targetState = QuestState.CAN_FINISH)
    {
        if (onAdvanceQuest != null)
        {
            onAdvanceQuest(id, targetState);
        }
    }

    public event Action<string> onFinishQuest;
    public void FinishQuest(string id)
    {
        if (onFinishQuest != null)
        {
            onFinishQuest(id);
        }
    }

    public event Action<Quest> onQuestStateChange;
    public void QuestStateChange(Quest quest)
    {
        if (onQuestStateChange != null)
        {
            onQuestStateChange(quest);
        }
    }

    // Quest steps removed: no per-step state change events
}

public class DialogueEvents
{
    public bool isInDialogue = false;

    public event Action<string> onEnterDialogue;
    public void EnterDialogue(string knotName)
    {
        isInDialogue = true;
        if (onEnterDialogue != null) onEnterDialogue(knotName);
    }

    public event Action onDialogueStarted;
    public void DialogueStarted()
    {
        if (onDialogueStarted != null) onDialogueStarted();
    }

    public event Action onDialogueFinished;
    public void DialogueFinished()
    {
        isInDialogue = false;
        if (onDialogueFinished != null)
        {
            onDialogueFinished();
        }
    }

    public event Action<string, List<Choice>, List<string>> onDisplayDialogue;
    public void DisplayDialogue(string dialogueLine, List<Choice> dialogueChoices, List<string> currentTags)
    {
        if (onDisplayDialogue != null) onDisplayDialogue(dialogueLine, dialogueChoices, currentTags);
    }

    public event Action<int> onUpdateChoiceIndex;
    public void UpdateChoiceIndex(int choiceIndex)
    {
        if (onUpdateChoiceIndex != null) onUpdateChoiceIndex(choiceIndex);
    }

    public event Action<string, Ink.Runtime.Object> onUpdateInkDialogueVariable;
    public void UpdateInkDialogueVariable(string name, Ink.Runtime.Object value)
    {
        if (onUpdateInkDialogueVariable != null) onUpdateInkDialogueVariable(name, value);
    }
}

public class BattleEvents
{
    public bool isBattleState = false;
    public Transform enemyInBattle;

    public event Action onBattleStarted;
    public void BattleStarted()
    {
        //      enemyInBattle.GetComponent<AutoMove>().isInBattle = true;
        isBattleState = true;
        onBattleStarted?.Invoke();
    }

    public event Action onBattleFinished;
    public void BattleFinished()
    {
        //     enemyInBattle.GetComponent<AutoMove>().isInBattle = false;
        isBattleState = false;
        onBattleFinished?.Invoke();
    }


    //   public event Action<int> onEnemyAttack;
    public void EnemyAttack(int damage)
    {
        Debug.Log("attack occured");
        // 플레이어의 PlayerHP에 있는 체력감소 함수 발동
    }
}

public class MiscEvents
{
    public event Action onCoinCollected;
    public void CoinCollected()
    {
        if (onCoinCollected != null)
        {
            onCoinCollected();
        }
    }

    public event Action onQuestExampleComplete;
    public void QuestExampleComplete()
    {
        if (onQuestExampleComplete != null)
        {
            onQuestExampleComplete();
        }
    }
}