using System;
using UnityEngine;
using UnityEngine.Windows.Speech;


/// <summary>
/// Player의 체력만을 처리하는 스크립트
/// 
/// HealthBarShrink에서 View를 처리합니다.
/// 
/// 
/// </summary>
public class PlayerHealth : MonoBehaviour, ISaveable<HealthData>
{
    [SerializeField] public float maxHealth = 100;
    [SerializeField] private DamageReaction damageReaction;

    public float CurrentHealth { get; private set; }
    public Action<float> OnDamaged;

    void Awake()
    {
        CurrentHealth = maxHealth;
    }

    private void Start()
    {
        OnDamaged += MainCameraScript.Instance.ShakeCamera;
    }

    private void OnDisable()
    {
        if(MainCameraScript.Instance != null)
            OnDamaged -= MainCameraScript.Instance.ShakeCamera;
    }

    public void TakeDamage(float damage, Vector2 direction = default, float force = 50f)
    {
        if (Player.Instance.isInvincible) return;

        SoundManager.Instance.PlaySFX("frontSlash2", 0.1f);

        OnDamaged?.Invoke(damage);
        damageReaction.Knockback(direction, force);
        damageReaction.StartBlinking();

        float totalDamage = (damage - Player.Instance.stats.flatDEF) * Player.Instance.stats.percentDEF;

        CurrentHealth = CurrentHealth - totalDamage < 0 ? 0 : CurrentHealth - totalDamage;
        if (CurrentHealth <= 0)
        {
            Die();
            return;
        }

    }

    public void Heal(int amount)
    {
        if (CurrentHealth <= 0) return;

        CurrentHealth = Mathf.Min(CurrentHealth + amount, maxHealth);
        Debug.Log($"Player healed by {amount}. Current Health: {CurrentHealth}");
    }

    public void ResetHealth()
    {
        CurrentHealth = maxHealth;
    }

    void Die()
    {
        Player.Instance.DieEffect();
    }

    public void Save(ref HealthData data)
    {
        data.health = CurrentHealth;
    }

    public void Load(HealthData data)
    {
        CurrentHealth = data.health;
    }
}
