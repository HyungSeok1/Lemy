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
        if (!CanExecute) return;

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
        Vector2 dir = (Camera.main.ScreenToWorldPoint(screen) - Player.Instance.transform.position);
        dir = dir.normalized;

        // 속도 초기화 & 가속
        Player.Instance.movement.skillVelocity = dir * data.dashPower;
        SoundManager.Instance.PlaySFX("dash1", 0.5f);

        //애니메이션 실행 
        Player.Instance.animator.SetTrigger("isDash");

        //이펙트 생성
        float angleRad = Mathf.Atan2(dir.y, dir.x);
        float angleDeg = angleRad * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angleDeg + 30f + 180f);

        GameObject dash = Instantiate(
            Player.Instance.DashEffect,
            transform.position,
            rotation
        );

        dash.transform.localScale *= 2f; // 크기를 2배로
    }

    public float GetNormalizedRemainingCooldown()
    {
        return remainingCooldown > 0 ? remainingCooldown / data.cooldown : 0;
    }
}
