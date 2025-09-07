-> 101_CollectKey

VAR collected_key_count = 0

=== 101_CollectKey ===
-> in_progress

= in_progress
~ collected_key_count = GetItemCount("Key")
{ collected_key_count:
    - 0:세 개의 방에서 세 개의 열쇠를 가져와라.
    그렇게 하지 못하는 자는 약하므로, 죽은 자들의 세계를 지나갈 수 없다.
    - 1:남은 두 개의 방에서 두 개의 열쇠를 가져와라.
    죽은 자들의 세계는 위험하므로, 네가 살아남을 수 있다는 것을 증명해야 한다.]
    - 2:남은 한 개의 방에서 한 개의 열쇠를 가져와라.
    네가 살아 있는 육신을 지니고도 안전할 수 있음을 증명하라.
    - 3: -> can_finish
}
-> END

= can_finish
너는 이 앞으로 나아갈 만큼 강인하구나.
그러나 네가 날 수 있고 적을 무찌를 수 있는 것은 내가 너에게 힘을 나눠 주었기 때문이다.
저 앞은 이곳보다 더 위험하므로, 네가 만나는 사람들에게 힘을 나눠 받도록 하거라.
그렇다면 가거라. 부디 살아남아 돌아오길 바라마.
//문열리고 문안으로 들어가는 컷신 혹은 함수 추가
-> END

= default_dialogue
문으로 들어가라
-> END