-> Map2RescueBrother1_2

=== Map2RescueBrother1_2 ===
{ QState("Map2RescueBrothers"):
    - "CAN_START": -> can_start
    - "IN_PROGRESS": -> in_progress
    - "CAN_FINISH": -> can_finish
    - else: -> Error
}

= can_start
~ AnimationChange("Map2RescueBrother1_2", "Talk", true)
#speaker: 쌍둥이2
들어오기 전까지는 저게 엄청 느리게 돌아가는 것 같았는데..<br>막상 들어오니까 너무 빨라서 나갈 수가 없잖아..
뭐? 구해 줄 필요 없어!<br> 이 나이 먹고 어린애한테 도움을 받을 수는 없다!
~ AnimationChange("Map2RescueBrother1_2", "Talk", false)
-> END

= in_progress
~ AnimationChange("Map2RescueBrother1_2", "Talk", true)
#speaker: 쌍둥이2
들어오기 전까지는 저게 엄청 느리게 돌아가는 것 같았는데..<br>막상 들어오니까 너무 빨라서 나갈 수가 없잖아..
뭐? 구해 줄 필요 없어! <br>이 나이 먹고 어린애한테 도움을 받을 수는 없다!
//(레미: whisper)
뭐? 누나가 찾고 있다고? <br>울고 있었단 말이야??
큰일났다! 먼저 가 볼게!
~ QSetInt("Map2RescueBrothers", "rescuedBrothersCount", QGetInt("Map2RescueBrothers", "rescuedBrothersCount") + 1)
-> END

= can_finish
~ AnimationChange("Map2RescueBrother1_2", "Talk", true)
#speaker: 쌍둥이2
누나 성질은 죽어도 안 가라앉는군…<br>이 말을 한 건 비밀이야. 
~ AnimationChange("Map2RescueBrother1_2", "Talk", false)
-> END

= Error
#speaker: 에러 메시지
뭔가 잘못된 것 같아...
-> END

