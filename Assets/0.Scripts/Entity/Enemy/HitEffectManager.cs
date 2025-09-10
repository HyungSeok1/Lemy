using UnityEngine;

public class HitEffectManager : MonoBehaviour
{
    [SerializeField] private float distance;

    [Header("히트 이펙트")]
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
        Quaternion rot = Quaternion.Euler(0, 0, angle -45f);

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


}
