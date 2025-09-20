using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class QuestManager : PersistentSingleton<QuestManager>
{
    [Header("Quest Management")]
    [Description("Pull all Quest Information Scriptable Objects.")]
    [SerializeField] private QuestInfoSO[] questInfos;
    private Dictionary<string, Quest> questMap;

    [SerializeField] private List<Quest> activeQuests = new List<Quest>();

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
    }

    private void OnDisable()
    {
        if (GameEventsManager.Instance is null) return;

        GameEventsManager.Instance.questEvents.onStartQuest -= StartQuest;
        GameEventsManager.Instance.questEvents.onAdvanceQuest -= AdvanceQuest;
        GameEventsManager.Instance.questEvents.onFinishQuest -= FinishQuest;
    }

    private void Start()
    {

        foreach (Quest quest in questMap.Values)
        {
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
        foreach (Quest quest in questMap.Values) // 필요 퀘스트가 완료되었는지 확인후 퀘스트 state 변경
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

        PopupUI.Instance.ShowQuestPopup(quest.info.displayName); 
        Debug.Log("Start: " + id);
        ChangeQuestState(quest.info.id, QuestState.IN_PROGRESS);
        InstantiateQuestPrefab(quest.info.id);
        activeQuests.Add(quest);
    }

    private void AdvanceQuest(string id, QuestState targetState = QuestState.CAN_FINISH)
    {
        Debug.Log("Advance: " + id + " to " + targetState);
        Quest quest = GetQuestById(id);
        ChangeQuestState(quest.info.id, targetState);
    }

    private void FinishQuest(string id)
    {
        Debug.Log("Finish: " + id);
        Quest quest = GetQuestById(id);
        ChangeQuestState(quest.info.id, QuestState.FINISHED);
        activeQuests.Remove(quest);
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
        if (quest == null)
        {
            Debug.LogError("Quest not found: " + id);
            return QuestState.REQUIREMENTS_NOT_MET;
        }
        return quest.state;
    }

    public GameObject InstantiateQuestPrefab(string id)
    {
        Quest quest = GetQuestById(id);
        if (quest == null)
        {
            Debug.LogError("Quest not found: " + id);
            return null;
        }

        try
        {
            GameObject questPrefabInstance = Instantiate(quest.info.questLogicPrefab);
            quest.questLogicInstance = questPrefabInstance.GetComponent<QuestLogic>();
            quest.questLogicInstance.Initialize(quest);
            return questPrefabInstance;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to instantiate quest prefab for quest '{id}': {e.Message}");
            return null;
        }
    }

    #region Quest Variable Functions
    private Quest TryGetQuest(string id)
    {
        if (string.IsNullOrEmpty(id) || questMap == null) return null;
        Quest q;
        return questMap.TryGetValue(id, out q) ? q : null;
    }

    public int QGetInt(string id, string key, int defaultValue = 0)
    {
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(key)) return defaultValue;
        Quest quest = TryGetQuest(id);
        return quest != null ? quest.GetIntVar(key, defaultValue) : defaultValue;
    }

    public void QSetInt(string id, string key, int value)
    {
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(key)) return;
        Quest quest = TryGetQuest(id);
        quest?.SetIntVar(key, value);
    }

    public int QAddInt(string id, string key, int delta)
    {
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(key)) return 0;
        Quest quest = TryGetQuest(id);
        return quest != null ? quest.AddIntVar(key, delta) : 0;
    }

    // Compare quest int variable with a value using an operator string
    // Operators supported: >,>=,<,<=,==,!=
    public bool QCheckInt(string id, string key, string op, int rhs)
    {
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(key)) return false;
        Quest quest = TryGetQuest(id);
        return quest != null && quest.CompareIntVar(key, op, rhs);
    }
    #endregion
}
