using UnityEngine;
using System;

public class BossBase : MonoBehaviour, IDamageable
{
    [SerializeField] public string bossName;
    [SerializeField] protected int maxHealth;

    public int MaxHealth
    {
        get => maxHealth;
    }

    /// <summary>
    /// 직접 건드리지 말고, TakeDamage 메서드를 통해서만 변경해주세요.
    /// </summary>
    private int health;
    public int Health
    {
        get => health;
    }


    public event Action OnHealthChanged;

    public virtual void Init()
    {
        health = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        OnHealthChanged?.Invoke();
        health = Mathf.Clamp(health - damage, 0, maxHealth);

        print(health);

        if (health <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        BossFightSystem.Instance.stateMachine.Change(BossFightState.Defeated);

    }

    #region BossFightSystem 관련 & FSM

    // BossFight FSM
    public virtual void Intro()
    {

    }


    public virtual void CleanupBehaviour()
    {
        Destroy(gameObject);
    }

    #endregion

}
