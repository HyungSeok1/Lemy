using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class QuestManager : PersistentSingleton<QuestManager>
{
    [Header("Quest Management")]
    [Description("Pull all Quest Information Scriptable Objects.")]
    [SerializeField] private QuestInfoSO[] questInfos;
    private Dictionary<string, Quest> questMap;


    protected override void Awake()
    {
        base.Awake();

        questMap = CreateQuestMap();
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.questEvents.onStartQuest += StartQuest;
        GameEventsManager.Instance.questEvents.onAdvanceQuest += AdvanceQuest;
        GameEventsManager.Instance.questEvents.onFinishQuest += FinishQuest;

        GameEventsManager.Instance.questEvents.onQuestStepStateChange += QuestStepStateChange;
    }

    private void OnDisable()
    {
        if (GameEventsManager.Instance is null) return;

        GameEventsManager.Instance.questEvents.onStartQuest -= StartQuest;
        GameEventsManager.Instance.questEvents.onAdvanceQuest -= AdvanceQuest;
        GameEventsManager.Instance.questEvents.onFinishQuest -= FinishQuest;
        GameEventsManager.Instance.questEvents.onQuestStepStateChange -= QuestStepStateChange;
    }

    private void Start()
    {

        foreach (Quest quest in questMap.Values)
        {
            // initialize any loaded quest steps
            if (quest.state == QuestState.IN_PROGRESS)
            {
                quest.InstantiateCurrentQuestStep(this.transform);
            }
            // broadcast the initial state of all quests on startup
            GameEventsManager.Instance.questEvents.QuestStateChange(quest);
        }
    }

    private void ChangeQuestState(string id, QuestState state)
    {
        Quest quest = GetQuestById(id);
        quest.state = state;
        GameEventsManager.Instance.questEvents.QuestStateChange(quest);
    }

    private bool CheckRequirementsmet(Quest quest)
    {
        bool meetsRequirements = true;

        foreach (QuestInfoSO prerequisteQuestInfo in quest.info.questPrerequisites)
        {
            if (GetQuestById(prerequisteQuestInfo.id).state != QuestState.FINISHED)
            {
                meetsRequirements = false;
            }
        }
        return meetsRequirements;
    }

    private void Update()
    {
        foreach (Quest quest in questMap.Values)
        {
            if (quest.state == QuestState.REQUIREMENTS_NOT_MET && CheckRequirementsmet(quest))
            {
                ChangeQuestState(quest.info.id, QuestState.CAN_START);
            }
        }
    }

    private void StartQuest(string id)
    {
        Quest quest = GetQuestById(id);
        if (quest.state != QuestState.CAN_START)
        {
            Debug.LogWarning("Quest cannot be started: " + id);
            return;
        }

        Debug.Log("Start: " + id);
        quest.InstantiateCurrentQuestStep(this.transform);
        ChangeQuestState(quest.info.id, QuestState.IN_PROGRESS);
    }

    private void AdvanceQuest(string id)
    {
        Debug.Log("Advance: " + id);
        Quest quest = GetQuestById(id);
        quest.MoveToNextStep();
        if (quest.CurrentStepExists())
        {
            quest.InstantiateCurrentQuestStep(this.transform);
        }
        else
        {
            ChangeQuestState(quest.info.id, QuestState.CAN_FINISH);
        }
    }

    private void FinishQuest(string id)
    {
        Debug.Log("Finish: " + id);
        Quest quest = GetQuestById(id);
        ChangeQuestState(quest.info.id, QuestState.FINISHED);
    }

    private void QuestStepStateChange(string id, int stepIndex, QuestStepState questStepState)
    {
        Quest quest = GetQuestById(id);
        quest.StoreQuestStepState(questStepState, stepIndex);
        ChangeQuestState(id, quest.state);
    }

    private Dictionary<string, Quest> CreateQuestMap()
    {
        Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();
        foreach (QuestInfoSO questInfo in questInfos)
        {
            if (idToQuestMap.ContainsKey(questInfo.id))
            {
                Debug.LogWarning("Duplicate ID found when creating quest map: " + questInfo.id);

            }
            idToQuestMap.Add(questInfo.id, new Quest(questInfo));
        }
        return idToQuestMap;
    }

    private Quest GetQuestById(string id)
    {
        Quest quest = questMap[id];
        if (quest == null)
        {
            Debug.LogError("ID not found in the Quest Map: " + id);

        }
        return quest;
    }

    public Dictionary<string, Quest> GetAllQuests()
    {
        return questMap;
    }

    public QuestState GetQuestState(string id)
    {
        Quest quest = GetQuestById(id);
        return quest?.state ?? QuestState.REQUIREMENTS_NOT_MET;
    }

    protected override void OnApplicationQuit()
    {
        foreach (Quest quest in questMap.Values)
        {
            QuestData questData = quest.GetQuestData();
            //Debug.Log("Qust.info.id");
            //Debug.Log("state = " + questData.state);
            //Debug.Log("index = " + questData.questStepIndex);
            //foreach (QuestStepState stepState in questData.questStepStates)
            //{
            //    Debug.Log("step state = " + stepState.state);
            //}
        }
    }
}
