using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map2RescueBrothersQuestLogic : QuestLogic
{
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
}