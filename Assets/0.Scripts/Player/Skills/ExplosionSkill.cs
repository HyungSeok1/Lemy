using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// ExplosionSkill class 사거리 내에서 폭발을 발생시키는 스킬
/// 폭발은 플레이어의 위치를 기준으로 마우스 위치에 따라 발생하며,
/// 폭발 반경 내의 적에게 데미지와 넉백 적용
/// </summary>
public class ExplosionSkill : MonoBehaviour, ISkill
{
    public ExplosionSkillData data;
    public SkillData skilldata => data;
    public float remainingCooldown;
    public bool CanExecute => remainingCooldown <= 0&& stackSystem.GetCurrentStackValue() >= data.stackGaugeCost;
    private StackSystem stackSystem;

    private GameObject rangeIndicator;

    private void Start()
    {
        stackSystem = Player.Instance.stackSystem;

    }
    private void Update()
    {
        if (remainingCooldown > 0)
            remainingCooldown -= Time.deltaTime;
    }

    public void ExecuteSkill()
    {
        if (!CanExecute) return;

        remainingCooldown = data.cooldown;
        PerformExplosion();
    }

    private void PerformExplosion()
    {
        // 스택 게이지 소모
        stackSystem.RemoveStackGauge(data.stackGaugeCost);

        SoundManager.Instance.PlaySFX("lightning2", 0.7f);
        // 마우스 위치를 월드 좌표로 가져오기
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));
        worldMousePosition.z = 0; // 폭발이 2D 평면에서 발생하도록 z값 설정

        // 플레이어의 위치 가져오기
        Vector3 playerPosition = transform.position;

        // 플레이어와 마우스 위치 간의 거리 계산
        float distanceToMouse = Vector3.Distance(playerPosition, worldMousePosition);

        // 폭발 위치 결정
        Vector3 explosionPosition;
        if (distanceToMouse <= data.castRadius)
        {
            // 마우스가 시전 반경 내에 있으면 마우스 위치 사용
            explosionPosition = worldMousePosition;
        }
        else
        {
            // 마우스가 시전 반경 밖에 있으면 위치를 시전 반경으로 제한
            Vector3 directionToMouse = (worldMousePosition - playerPosition).normalized;
            explosionPosition = playerPosition + directionToMouse * data.castRadius;
        }

        // 결정된 위치에 폭발 효과 생성
        GameObject explosionEffect = Instantiate(data.explosionEffectPrefab, explosionPosition, Quaternion.identity);
        explosionEffect.GetComponent<ExplosionEffect>().data = data;
        print("Explosion 사용됨.");
    }

    public float GetNormalizedRemainingCooldown()
    {
        return remainingCooldown / data.cooldown;
    }

    public void InitializeSkill()
    {
        remainingCooldown = data.cooldown;
        if (data.rangePrefab != null)
        {
            rangeIndicator = Instantiate(data.rangePrefab, transform);
            rangeIndicator.transform.position = transform.position;
            rangeIndicator.transform.localScale = new Vector3(data.castRadius / 4.5f, data.castRadius / 4.5f, 1); // 폭발 범위 표시를 위한 스케일 조정
        }
    }

    public void ReleaseSkill()
    {
        if (rangeIndicator != null)
        {
            Destroy(rangeIndicator);
        }
    }
}
