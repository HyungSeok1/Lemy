-> MultiStepQuestExample

=== MultiStepQuestExample ===
// 다단계 퀘스트 예시
{MultiStepQueststate:
    - "REQUIREMENTS_NOT_MET": -> requirements_not_met
    - "CAN_START": -> can_start
    - "IN_PROGRESS": -> check_progress
    - "CAN_FINISH": -> can_finish
    - "FINISHED": -> finished
    - else: -> default_dialogue
}

= requirements_not_met
아직 이 퀘스트를 시작할 수 없습니다. #speaker: 연구자 #portrait: Researcher_thinking
다른 실험을 먼저 완료해야 합니다.
-> END

= can_start
흥미로운 실험에 참여해보시겠습니까? #speaker: 연구자 #portrait: Researcher_excited
여러 단계의 작업을 수행해야 합니다.
    * [네, 참여하겠습니다]
        좋습니다! 첫 번째 단계부터 시작해봅시다. #speaker: 연구자 #portrait: Researcher_happy
        ~ StartQuest("MultiStepQuest")
        -> END
    * [다음에 하겠습니다]
        언제든 준비되시면 와주세요. #speaker: 연구자 #portrait: Researcher_neutral
        -> END

= check_progress
실험은 어떻게 진행되고 있나요? #speaker: 연구자 #portrait: Researcher_curious

// 현재 단계에 따른 대화 분기
// 이 부분은 QuestStep의 상태에 따라 달라질 수 있습니다
    * [진행 상황 보고]
        계속 진행해주세요. 다음 단계로 넘어갈 준비가 되면 알려드리겠습니다. #speaker: 연구자 #portrait: Researcher_encouraging
        -> END
    * [다음 단계로 진행]
        // 현재 단계가 완료되었다면 다음 단계로 진행
        좋습니다! 다음 단계를 시작하겠습니다. #speaker: 연구자 #portrait: Researcher_pleased
        ~ AdvanceQuest("MultiStepQuest")
        -> END
    * [도움이 필요합니다]
        어떤 부분에서 막히셨나요? 힌트를 드리겠습니다. #speaker: 연구자 #portrait: Researcher_helpful
        -> give_hint

= give_hint
현재 단계에서는... #speaker: 연구자 #portrait: Researcher_explaining
// 여기에 힌트 내용 추가
잘 따라와 주세요!
-> END

= can_finish
훌륭합니다! 모든 단계를 완료하셨네요! #speaker: 연구자 #portrait: Researcher_amazed
실험이 성공적으로 끝났습니다.
    * [실험 완료하기]
        정말 대단합니다! 이 결과는 매우 중요한 발견입니다. #speaker: 연구자 #portrait: Researcher_ecstatic
        ~ FinishQuest("MultiStepQuest")
        -> END

= finished
당신 덕분에 중요한 연구를 완료할 수 있었습니다. #speaker: 연구자 #portrait: Researcher_grateful
언제든 다른 실험이 있으면 연락드리겠습니다!
-> END

= default_dialogue
무언가 예상치 못한 일이 일어났네요... #speaker: 연구자 #portrait: Researcher_confused
시스템을 다시 확인해보겠습니다.
-> END
