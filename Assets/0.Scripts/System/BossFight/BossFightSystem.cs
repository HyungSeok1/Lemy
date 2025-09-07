using UnityEngine;
using UnityEngine.Playables;

public enum BossFightState
{
    None,
    Intro,
    Fight,
    Defeated,
    Cleanup
}
/// <summary>
/// 보스 체력바, 텍스트 등 관리
/// <para> 보스의 부가 효과(ex. 보스방 들어가면 문 닫는 것)들은 보스 스크립트 내에서 관리.</para>
/// </summary>
public class BossFightSystem : PersistentSingleton<BossFightSystem>
{
    [HideInInspector] public PlayableDirector introDirector;
    [HideInInspector] public BossBase boss;
    [HideInInspector] public BossUI bossUI;


    public StateMachine<BossFightState> stateMachine { get; private set; } = new();
    public BossFightState Current => stateMachine.Current;

    protected override void Awake()
    {
        base.Awake();

        stateMachine.Register(BossFightState.None, new None(this));
        stateMachine.Register(BossFightState.Intro, new Intro(this));
        stateMachine.Register(BossFightState.Fight, new Fight(this));
        stateMachine.Register(BossFightState.Defeated, new Defeated(this));
        stateMachine.Register(BossFightState.Cleanup, new Cleanup(this));
        stateMachine.Change(BossFightState.None);
    }

    private void Start()
    {
        bossUI = BossUI.Instance;
    }

    private void Update()
    {
        stateMachine.Tick(Time.deltaTime);
    }

    public void RegisterBoss(BossBase bossRef)
    {
        boss = bossRef;
    }
}

// 각 상태 구현 
class None : IState
{
    readonly BossFightSystem bossFightSystem;
    public None(BossFightSystem c) => bossFightSystem = c;
    public void OnEnter() { }
    public void OnExit() { }
    public void Tick(float dt) { }
}
class Intro : IState
{
    readonly BossFightSystem bossFightSystem;
    public Intro(BossFightSystem c) => bossFightSystem = c;
    public void OnEnter()
    {
        bossFightSystem.boss.Intro();
    }
    public void OnExit() { }
    public void Tick(float dt) { }
}
class Fight : IState
{
    readonly BossFightSystem bossFightSystem;
    public Fight(BossFightSystem c) => bossFightSystem = c;
    public void OnEnter()
    {
        bossFightSystem.bossUI.OnStartBossFight(bossFightSystem.boss); //정보 넣어주기 & 켜주기
    }
    public void OnExit() { }
    public void Tick(float dt) { }
}
class Defeated : IState
{
    readonly BossFightSystem bossFightSystem;
    float _timer = 0f;
    public Defeated(BossFightSystem c) => bossFightSystem = c;
    public void OnEnter()
    {
    }
    public void OnExit() { }
    public void Tick(float dt)
    {
        _timer += dt;
        if (_timer > 1.5f) bossFightSystem.stateMachine.Change(BossFightState.Cleanup); // 예시. 1.5초후 끝나게 함
    }
}
class Cleanup : IState
{
    readonly BossFightSystem bossFightSystem;
    public Cleanup(BossFightSystem c) => bossFightSystem = c;
    public void OnEnter()
    {
        bossFightSystem.boss.CleanupBehaviour();
        bossFightSystem.bossUI.OnEndBossFight();
        bossFightSystem.boss = null;
    }
    public void OnExit() { }
    public void Tick(float dt) { }
}