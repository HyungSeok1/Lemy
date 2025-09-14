using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 실제 Quest 작동 로직을 담당하는 클래스.
/// </summary>
public class QuestLogic : MonoBehaviour
{
    public Quest quest;

    public virtual void Initialize(Quest quest)
    {
        this.quest = quest;

        //자식 클래스에서 오버라이드하여 각 변수를 quest.AddIntVar로 추가해 줄 것
    }

    /// <summary>
    /// 퀘스트의 정수 변수 값이 변경될 때 호출
    /// </summary>
    public virtual void OnIntVarChanged(string key, int oldValue, int newValue)
    {
        // override in subclasses to react to variable changes
    }
}