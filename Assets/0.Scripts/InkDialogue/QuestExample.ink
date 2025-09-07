-> QuestExample

=== QuestExample ===
{QuestExamplestate:
    - "REQUIREMENTS_NOT_MET": -> requirements_not_met
    - "CAN_START": -> can_start
    - "IN_PROGRESS": -> in_progress  
    - "CAN_FINISH": -> can_finish
    - "FINISHED": -> finished
    - else: -> default_dialogue
}

= requirements_not_met
아직 이 퀘스트를 받을 조건이 되지 않습니다.
다른 퀘스트를 먼저 완료해 주세요.
-> END

= can_start
안녕하세요! 제가 부탁드릴 일이 하나 있습니다.
마을에 떨어진 동전들을 모아주실 수 있나요?
    * [네, 도와드리겠습니다!]
        감사합니다! 동전 10개를 모아주시면 됩니다.
        ~ StartQuest("QuestExample")
        -> END
    * [죄송하지만 지금은 바빠서...]
        괜찮습니다. 시간이 나실 때 다시 와주세요.
        -> END

= in_progress
동전 모으기는 어떻게 진행되고 있나요? 
아직 모두 찾지 못하셨다면 계속 찾아보세요!
    * [계속 찾아보겠습니다]
        힘내세요! 
        -> END
    * [퀘스트를 포기하고 싶습니다]
        // 퀘스트 포기 기능이 필요하다면 여기에 추가
        아쉽네요... 마음이 바뀌시면 언제든 말씀해 주세요. 
        -> END

= can_finish
오! 동전을 모두 모으셨군요!
정말 감사합니다. 약속드린 보상을 드리겠습니다.
    * [완료하기]
        수고하셨습니다! 이것은 감사의 표시입니다. 
        ~ FinishQuest("QuestExample")
        -> END

= finished
다시 한 번 감사드립니다!
덕분에 큰 도움이 되었습니다.
-> END

= default_dialogue
무언가 잘못된 것 같네요... 
-> END
