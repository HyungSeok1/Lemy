using UnityEngine;

public class BossSample : BossBase
{
    private void Awake()
    {
        Init();
    }

    /// <summary>
    /// 보스 시작전 실행되는 메서드 ( None-> Intro 전환시 바로 실행)
    /// </summary>
    public override void Intro()
    {
        base.Intro();

        // 뭔가가 다 끝나면 바꾸기 (코루틴 실행)
        BossFightSystem.Instance.stateMachine.Change(BossFightState.Fight);
    }

    // 기본적으로 Defeat (상속받음)
    protected override void Die()
    {
        base.Die();
    }



    // TODO: 보스 로직 등 구현

}
