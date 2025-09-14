using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Ink.Parsed;

public class Map2RescueBrothersQuestLogic : QuestLogic
{
    private GameObject starterNPC;
    private bool isGathered = false;

    void Start()
    {
        starterNPC = GameObject.Find("Map2RescueStarter");
    }
    void Update()
    {
        if (quest.state == QuestState.CAN_FINISH && !isGathered)
        {
            GatherBrothers();
        }

    }
    public int rescuedBrothersCount
    {
        get { return quest.GetIntVar("rescuedBrothersCount", 0); }
        set
        {
            quest.SetIntVar("rescuedBrothersCount", value);

        }
    }

    public override void Initialize(Quest quest)
    {
        base.Initialize(quest);

        quest.AddIntVar("rescuedBrothersCount", 0);
    }

    public override void OnIntVarChanged(string key, int oldValue, int newValue)
    {
        if (key == "rescuedBrothersCount")
        {
            if (newValue == 3 && quest.state == QuestState.IN_PROGRESS)
            {
                GameEventsManager.Instance.questEvents.AdvanceQuest(quest.info.id, QuestState.CAN_FINISH);
            }
        }
    }

    private void GatherBrothers()
    {
        isGathered = true;

        Transform brotherTransform = starterNPC.transform.Find("Map2RescueBrother1_1");
        if (brotherTransform != null)
        {
            brotherTransform.gameObject.SetActive(true);
        }

        brotherTransform = starterNPC.transform.Find("Map2RescueBrother1_2");
        if (brotherTransform != null)
        {
            brotherTransform.gameObject.SetActive(true);
        }

        brotherTransform = starterNPC.transform.Find("Map2RescueBrother2");
        if (brotherTransform != null)
        {
            brotherTransform.gameObject.SetActive(true);
            brotherTransform.GetComponent<Animator>().SetBool("Idle2", true);
        }

        starterNPC.GetComponent<Animator>().SetBool("Happy", true);
    }
}