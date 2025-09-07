=== startPoint ===
{CollectCoinsQueststate : 
    - "REQUIREMENTS_NOT_MET": -> requirementsNotMet
    - "CAN_START": -> canStart
    - "IN_PROGRESS": -> inProgress
    - "CAN_FINISH": -> canFinish
    - "FINISHED": -> finished
    - else: -> END
}

= requirementsNotMet
Requirements not met. #speaker: Capsuleman #portrait: Capsuleman_happy
-> END
= canStart
You can start the quest. #speaker: Capsuleman #portrait: Capsuleman_happy
Will you? #portrait: Capsuleman_happy2
    * [Yes]
    Good! # speaker: Capsulewoman
    ~ StartQuest(CollectCoinsQuestId)
    -> END
    * [No]
    Oh. Rain check then.
    -> END
= inProgress
inProgress. #speaker: Capsuleman #portrait: Capsuleman_happy
-> END
= canFinish
canFinish. #speaker: Capsuleman #portrait: Capsuleman_happy
-> END
= finished
finished. #speaker: Capsuleman #portrait: Capsuleman_happy
-> END
