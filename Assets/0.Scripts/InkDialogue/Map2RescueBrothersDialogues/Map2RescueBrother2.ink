-> Map2RescueBrother2

=== Map2RescueBrother2 ===
{ QState("Map2RescueBrothers"):
    - "CAN_START": -> can_start
    - "IN_PROGRESS": -> in_progress
    - "CAN_FINISH": -> can_finish
    - else: -> Error
}

= can_start
#speaker: 막내
우애애애애앵… 좁아서 나갈 수가 없어…
-> END

= in_progress
#speaker: 막내
우애애애애앵… 좁아서 나갈 수가 없…
//(문 열림)
뭐야. 저거 열리는 거였어?<br>크흠. 고맙다 꼬맹아.
~ QSetInt("Map2RescueBrothers", "rescuedBrothersCount", QGetInt("Map2RescueBrothers", "rescuedBrothersCount") + 1)
-> END

= can_finish
#speaker: 막내
우리 누나는 너무 무서워.
-> END

= Error
#speaker: 에러 메시지
뭔가 잘못된 것 같아...
-> END

