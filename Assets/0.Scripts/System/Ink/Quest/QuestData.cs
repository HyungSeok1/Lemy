using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestData
{
    public QuestState state;
    public QuestData(QuestState state)
    {
        this.state = state;
    }
}