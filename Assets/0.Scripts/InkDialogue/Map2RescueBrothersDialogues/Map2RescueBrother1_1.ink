-> Map2RescueBrother1_1

=== Map2RescueBrother1_1 ===
{ QState("Map2RescueBrothers"):
    - "CAN_START": -> can_start
    - "IN_PROGRESS": -> in_progress
    - "CAN_FINISH": -> can_finish
    - else: -> Error
}

= can_start
#speaker: 쌍둥이1
으으으… 저 빙빙 돌아가는 게 생각보다 너무 빨라.
도저히 나갈 타이밍을 못 잡겠어…
-> END

= in_progress
#speaker: 쌍둥이1
으으으… 저 빙빙 돌아가는 게 생각보다 너무 빨라.<br>도저히 나갈 타이밍을 못 잡겠어…
//(레미: whisper)
뭐? 누나가 날 찾고 있어?<br>거기다 울고 있었다고?
난 죽었구나! 이미 죽긴 했지만!! <br>당장 돌아가야겠어!!
//(사라짐)
~ QSetInt("Map2RescueBrothers", "rescuedBrothersCount", QGetInt("Map2RescueBrothers", "rescuedBrothersCount") + 1)
-> END

= can_finish
#speaker: 쌍둥이1
뭐.. 마음대로 돌아다녔다길래 완전 어린애일 줄 알았어?<br>고등학생도 호기심이 많을 수 있는 거잖아!
-> END

= Error
#speaker: 에러 메시지
뭔가 잘못된 것 같아...
-> END

