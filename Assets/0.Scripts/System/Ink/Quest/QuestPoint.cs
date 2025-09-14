using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class QuestPoint : MonoBehaviour
{
    [Header("Dialogue (optional)")]
    [SerializeField] private string dialogueKnotName;

    [Header("Quest")]
    [SerializeField] private QuestInfoSO questInfoForPoint;

    [Header("Config")]
    [SerializeField] private bool startPoint = true;
    [SerializeField] private bool finishPoint = true;

    private bool playerIsNear = false;
    private string questId;
    private QuestState currentQuestState;
    private QuestIcon questIcon;

    private void Awake()
    {
        questId = questInfoForPoint.id;
        questIcon = GetComponentInChildren<QuestIcon>();
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.questEvents.onQuestStateChange += QuestStateChange;
        GameEventsManager.Instance.inputEvents.onSubmitPressed += SubmitPressed;
    }
    private void OnDisable()
    {
        
        GameEventsManager.Instance.questEvents.onQuestStateChange -= QuestStateChange;
        GameEventsManager.Instance.inputEvents.onSubmitPressed -= SubmitPressed;
    }
    


    private void QuestStateChange(Quest quest)
    {
        if (quest.info.id.Equals(questId))
        {
            currentQuestState = quest.state;
            questIcon.SetState(currentQuestState, startPoint, finishPoint);
        }
    }

    private void SubmitPressed(InputEventContext inputEventContext)
    {
        if (CutsceneManager.Instance.isCutscenePlaying) return;

        if (!playerIsNear || !inputEventContext.Equals(InputEventContext.DEFAULT))
        {
            return;
        }

        // if we have a knot name defined, try to start dialogue with it
        if (!dialogueKnotName.Equals(""))
        {
            GameEventsManager.Instance.dialogueEvents.EnterDialogue(dialogueKnotName);
        }
        // otherwise, start or finish the quest immediately without dialogue
        else
        {
            // start or finish a quest
            if (currentQuestState.Equals(QuestState.CAN_START) && startPoint)
            {
                GameEventsManager.Instance.questEvents.StartQuest(questId);
            }
            else if (currentQuestState.Equals(QuestState.CAN_FINISH) && finishPoint)
            {
                GameEventsManager.Instance.questEvents.FinishQuest(questId);
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.CompareTag("Player"))
        {
            playerIsNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D otherCollider)
    {
        if (otherCollider.CompareTag("Player"))
        {
            playerIsNear = false;
        }
    }
}
