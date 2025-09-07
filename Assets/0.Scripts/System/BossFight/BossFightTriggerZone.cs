using UnityEngine;

/// <summary>
/// 들어가면 바로 Intro 시작되는 구역
/// </summary>
public class BossFightTriggerZone : MonoBehaviour
{
    [SerializeField] private BossBase boss;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        BossFightSystem.Instance.RegisterBoss(boss);
        BossFightSystem.Instance.stateMachine.Change(BossFightState.Intro);
    }
}
