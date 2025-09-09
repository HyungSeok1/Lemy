using System;
using System.Collections.Generic;
using UnityEngine;


public class MonsterChallenge : ChallengeZone
{
    [SerializeField] private EntityGuide cocoon;
    private Cocoon cocoonRef;
    [SerializeField] private List<EntityGuide> enemies; // 안의 flag는 무시. 그냥 재사용한거임 
    [SerializeField] private GameObject enemyParent;

    public override void Challenge()
    {
        base.Challenge();
        SpawnEnemies();
    }

    public override void BeatChallenge()
    {
        base.BeatChallenge();
    }

    // 적, cocoon 생성하고 싸움 시작
    private void SpawnEnemies()
    {
        foreach (var enemy in enemies)
        {
            var b = Instantiate(enemy.prefab, enemy.position, Quaternion.identity, enemyParent.transform);

            b.GetComponentInChildren<Enemy>().OnDie += OnEnemyDie;
        }
        enemiesRemaining = enemies.Count;

        // 코쿤 생성
        var a = Instantiate(cocoon.prefab, cocoon.position, Quaternion.identity, enemyParent.transform);
        a.SetActive(true);
        cocoonRef = a.GetComponent<Cocoon>();
    }

    private int enemiesRemaining;
    private void OnEnemyDie()
    {
        enemiesRemaining--;
        if (enemiesRemaining <= 0)
            EnemyAllKillCheck();
    }

    // 됐으면 오픈
    public void EnemyAllKillCheck()
    {
        cocoonRef.Open(BeatChallenge);
    }
}
