//함수 바인딩(InkExternalFunctions.cs 의 Bind된 함수와 동일해야함.)
//Quest
EXTERNAL StartQuest(questId)
EXTERNAL AdvanceQuest(questId)
EXTERNAL AdvanceQuestToState(questId, targetState)
EXTERNAL FinishQuest(questId)
//Quest State & Variables
EXTERNAL QState(questId)
EXTERNAL QGetInt(questId, key)
EXTERNAL QSetInt(questId, key, value)
EXTERNAL QAddInt(questId, key, delta)
EXTERNAL QCheckInt(questId, key, op, rhs)
//Animation
EXTERNAL AnimationChange(objectNPC, animationVariable, active)
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
//ETC
EXTERNAL DestroyNPC(objectNPC)

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

//Map2
INCLUDE Map2RescueBrothersDialogues\Map2RescueStarter.ink
INCLUDE Map2RescueBrothersDialogues\Map2RescueBrother1_1.ink
INCLUDE Map2RescueBrothersDialogues\Map2RescueBrother1_2.ink
INCLUDE Map2RescueBrothersDialogues\Map2RescueBrother2.ink
INCLUDE npc_talk_Map2_D.ink
INCLUDE npc_talk_Map2_E.ink
INCLUDE npc_talk_Map2_F.ink
