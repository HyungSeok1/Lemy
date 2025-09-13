//함수 바인딩(InkExternalFunctions.cs 의 Bind된 함수와 동일해야함.)
//Quest
EXTERNAL StartQuest(questId)
EXTERNAL AdvanceQuest(questId)
EXTERNAL FinishQuest(questId)
//Animation
EXTERNAL AnimationChange(objectNPC, animationVariable)
//Skill
EXTERNAL GiveSkill(skillName)
//Item
EXTERNAL GiveItem(itemName, count)
EXTERNAL TryConsumeItem(itemName, count)
EXTERNAL GetItemCount(itemID)
EXTERNAL HasItem(itemID, count)
//Money
EXTERNAL AddMoney(amount)
EXTERNAL SpendMoney(amount)
EXTERNAL HasEnoughMoney(amount)
EXTERNAL GetMoney()

VAR CollectCoinsQuestId = "CollectCoinsQuest"
VAR CollectCoinsQueststate = "REQUIREMENTS_NOT_MET"
VAR QuestExamplestate = "CAN_START"
VAR AdvancedQueststate = "REQUIREMENTS_NOT_MET"
VAR MultiStepQueststate = "REQUIREMENTS_NOT_MET"

INCLUDE startPoint.ink
INCLUDE finishPoint.ink
INCLUDE GreenCapsule.ink
INCLUDE NPCExample.ink
INCLUDE NPCTEST.ink
INCLUDE QuestExample.ink
INCLUDE ChainQuestExample.ink
INCLUDE MultiStepQuestExample.ink
INCLUDE ForgottenChamber.ink
INCLUDE Map1_Intro.ink
INCLUDE NPCRescueBasic.ink
INCLUDE 101_CollectKey.ink
INCLUDE npc_talk_Map2_D.ink
INCLUDE npc_talk_Map2_E.ink
INCLUDE npc_talk_Map2_F.ink