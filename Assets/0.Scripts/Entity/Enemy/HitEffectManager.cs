using System.Collections;
using UnityEngine;

/// <summary>
/// Enemy 및 Player의 피격연출 관리
/// </summary>
public class HitEffectManager : PersistentSingleton<HitEffectManager>
{
    [SerializeField] private float distance;

    [SerializeField] private GameObject HitEffect1;
    [SerializeField] private GameObject HitEffect2;
    [SerializeField] private GameObject HitEffect3;


    private void OnEnable()
    {
        Enemy.OnEnemyDamaged += HitEffect;
    }
    private void OnDisable()
    {
        Enemy.OnEnemyDamaged -= HitEffect;
    }


    private void HitEffect(Vector3 playerPos, Vector3 enemyPos)
    {
        int ran = Random.Range(0, 3);

        Vector2 dir = -(new Vector2(playerPos.x, playerPos.y) - new Vector2(enemyPos.x, enemyPos.y)).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0, 0, angle - 45f);

        Vector3 spawnPos = enemyPos + new Vector3(dir.x, dir.y, 0) * distance; ;


        switch (ran)
        {
            case 0:
                Instantiate(HitEffect1, spawnPos, rot);
                break;
            case 1:
                Instantiate(HitEffect2, spawnPos, rot);
                break;
            case 2:
                Instantiate(HitEffect3, spawnPos, rot);
                break;
        }
    }

    public void DoHitStop(float duration)
    {
        StartCoroutine(HitStopCoroutine(duration));
    }

    private IEnumerator HitStopCoroutine(float duration)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);

        if (!Pause.Instance.isPaused)
            Time.timeScale = 1f;
    }
}
