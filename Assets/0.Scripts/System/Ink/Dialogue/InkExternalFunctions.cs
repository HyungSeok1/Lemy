using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class InkExternalFunctions
{
    public void Bind(Story story)
    {
        //Quest
        story.BindExternalFunction("StartQuest", (string questId) => GameEventsManager.Instance.questEvents.StartQuest(questId));
        story.BindExternalFunction("AdvanceQuest", (string questId) => GameEventsManager.Instance.questEvents.AdvanceQuest(questId));
        story.BindExternalFunction("FinishQuest", (string questId) => GameEventsManager.Instance.questEvents.FinishQuest(questId));

        //Animation
        story.BindExternalFunction("AnimationChange", (string objectNPC, string animationVariable) => AnimationChange(objectNPC, animationVariable));

        //Skill
        story.BindExternalFunction("GiveSkill", (string skillName) => Player.Instance.playerSkillController.AddSkill(skillName));

        //Item
        story.BindExternalFunction("GiveItem", (string itemName, int count) => Player.Instance.inventory.AddItem(itemName, count));
        story.BindExternalFunction("TryConsumeItem", (string itemID, int count) => Player.Instance.inventory.TryConsume(itemID, count));
        story.BindExternalFunction("GetItemCount", (string itemID) => Player.Instance.inventory.GetItemCount(itemID));
        story.BindExternalFunction("HasItem", (string itemID, int count) => Player.Instance.inventory.HasItem(itemID, count));

        //Money
        story.BindExternalFunction("AddMoney", (int amount) => MoneyManager.Instance.AddMoney(amount));
        story.BindExternalFunction("SpendMoney", (int amount) => MoneyManager.Instance.SpendMoney(amount));
        story.BindExternalFunction("HasEnoughMoney", (int amount) => MoneyManager.Instance.HasEnoughMoney(amount));
        story.BindExternalFunction("GetMoney", () => MoneyManager.Instance.Money);
    }

    public void Unbind(Story story)
    {
        //Quest
        story.UnbindExternalFunction("StartQuest");
        story.UnbindExternalFunction("AdvanceQuest");
        story.UnbindExternalFunction("FinishQuest");

        //Animation
        story.UnbindExternalFunction("AnimationChange");

        //SKill
        story.UnbindExternalFunction("GiveSkill");

        //Item
        story.UnbindExternalFunction("GiveItem");
        story.UnbindExternalFunction("TryConsumeItem");
        story.UnbindExternalFunction("GetItemCount");
        story.UnbindExternalFunction("HasItem");

        //Money
        story.UnbindExternalFunction("AddMoney");
        story.UnbindExternalFunction("SpendMoney");
        story.UnbindExternalFunction("HasEnoughMoney");
        story.UnbindExternalFunction("GetMoney");
    }

    /// <summary>
    /// 애니메이션 상태를 변경합니다.
    /// </summary>
    /// <param name="objectNPC">NPC 오브젝트 이름</param>
    /// <param name="animationVariable">변경할 애니메이션 변수의 이름</param>
    private void AnimationChange(string objectNPC, string animationVariable)
    {
        GameObject NPC = GameObject.Find(objectNPC);
        if (NPC != null)
        {
            Animator animator = NPC.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool(animationVariable, true); // true 또는 false는 필요에 따라 조정
            }
            else
            {
                Debug.LogWarning($"Animator 컴포넌트를 {objectNPC}에서 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning($"GameObject '{objectNPC}'를 찾을 수 없습니다.");
        }
    }
}