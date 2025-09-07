using DG.Tweening;
using System.Collections;
using UnityEngine;

public class DashSkill : MonoBehaviour, ISkill
{
    public bool CanExecute => remainingCooldown <= 0;

    public SkillData skilldata => data;
    public DashSkillData data;

    private float remainingCooldown;
    private void Update()
    {
        if (remainingCooldown > 0)
            remainingCooldown -= Time.deltaTime;
    }


    public void ExecuteSkill()
    {
        //애니메이션 실행 
        if (!CanExecute) return;
        Player.Instance.animator.SetTrigger("isDash");
        float angle = Mathf.Atan2(Player.Instance.movement.dir.y, Player.Instance.movement.dir.x) * Mathf.Rad2Deg;
        Instantiate(Player.Instance.DashEffect, transform.position, Quaternion.Euler(0f, 0f, angle+30f));

        remainingCooldown = data.cooldown;
        Dash();
    }

    public void InitializeSkill()
    {
        remainingCooldown = data.cooldown;
    }


    public void ReleaseSkill()
    { }


    private void Dash()
    {
        // 회전
        Vector2 pointer = Player.Instance.currentMousePosition;
        Vector3 screen = new Vector3(pointer.x, pointer.y, Camera.main.nearClipPlane);
        Vector2 dir = (Camera.main.ScreenToWorldPoint(screen) - Player.Instance.transform.position).normalized;

        // 속도 초기화 & 가속
        Player.Instance.movement.skillVelocity = dir * data.dashPower;
        SoundManager.Instance.PlaySFX("dash1", 0.05f);
    }

    public float GetNormalizedRemainingCooldown()
    {
        return remainingCooldown > 0 ? remainingCooldown / data.cooldown : 0;
    }
}
