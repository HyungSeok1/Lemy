-> Map2RescueStarter

=== Map2RescueStarter ===
{ QState("Map2RescueBrothers"):
    - "CAN_START": -> can_start
    - "IN_PROGRESS": -> in_progress
    - "CAN_FINISH": -> can_finish
    - else: -> Error
}

= can_start
~ AnimationChange("Map2RescueStarter", "Talk", true)
#speaker: 누나
흑흑.. 다 어디로 사라진 거야…
훌쩍.. 우리 동생들은 워낙 활발해서... <br>이런 무서운 곳까지 와서도 제멋대로 돌아다니다가 사라져 버렸어.
혹시.. 내 동생들을 발견하면 여기로 데려와 줄래? <br>난 애들이 혹시 돌아올지도 몰라서 여기 있어야 할 것 같아..
~ AnimationChange("Map2RescueStarter", "Talk", false)
~ StartQuest("Map2RescueBrothers")
-> END

= in_progress
{ QGetInt("Map2RescueBrothers", "rescuedBrothersCount"):
    - 0: -> default_dialogue
    - 1: -> some_brothers_rescued
    - 2: -> some_brothers_rescued
    - 3: -> can_finish
}
= default_dialogue
#speaker: 누나
~ AnimationChange("Map2RescueStarter", "Talk", true)
훌쩍....내 동생들을 발견하면 데려와 줘.<br>모두 3명이야.
너무 바쁘면 그냥 가도 괜찮아…<br>내 가슴이 찢어지겠지만…
~ AnimationChange("Map2RescueStarter", "Talk", false)
-> END

= some_brothers_rescued
#speaker: 누나
~ AnimationChange("Map2RescueStarter", "Happy", true)
진짜로 동생을 찾아 줬구나! 정말정말 고마워!
얘는 다시는 혼자 안 돌아다니도록 따끔하게 교육시킬 거야! 
다시 한번 고마워. 다른 애들이 어디 있을지가 걱정이지만…
~ AnimationChange("Map2RescueStarter", "Happy", false)
-> END

= can_finish
#speaker: 누나
~ AnimationChange("Map2RescueStarter", "Happy", true)
우리 동생들을 전부 찾아 주다니.. <br>이 은혜를 어떻게 갚아야 하지?
내가 줄 건 이것밖에 없네. 뭔가 더 주고 싶은데, <br>우리도 여기 너무 갑작스럽게 와서 딱히 가진 게 없어. 미안..
~ GiveSkill("Explosion")
정말 고마웠어! 그럼 우린 다시 가 볼게! <br>나중에 또 만날 수 있으면 좋겠다!
~ FinishQuest("Map2RescueBrothers")
~ DestroyNPC("Map2RescueStarter")
-> END

= Error
#speaker: 에러 메시지
뭔가 잘못된 것 같아...
-> END

