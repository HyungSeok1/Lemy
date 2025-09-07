=== finishPoint ===
{CollectCoinsQueststate : 
    - "REQUIREMENTS_NOT_MET": -> requirementsNotMet
    - "CAN_START": -> canStart
    - "IN_PROGRESS": -> inProgress
    - "CAN_FINISH": -> canFinish
    - "FINISHED": -> finished
    - else: -> END
}

= requirementsNotMet
Bring out the coins. #speaker: Capsuleman #portrait: Capsuleman_happy
-> END
= canStart
Bring out the coins. #speaker: Capsuleman #portrait: Capsuleman_happy
-> END
= inProgress
Bring out the coins. #speaker: Capsuleman #portrait: Capsuleman_happy
-> END
= canFinish
Thanks. #speaker: Capsuleman #portrait: Capsuleman_happy
~ FinishQuest(CollectCoinsQuestId)
-> END
= finished
Thanks again. #speaker: Capsuleman #portrait: Capsuleman_happy
-> END
