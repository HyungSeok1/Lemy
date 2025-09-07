using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monster;
    public int maxMonster;
    public float spawnTimer;

    int curMonster;
    float time = 0f;

    void Update()
    {
        curMonster = transform.childCount;
        if (curMonster > maxMonster - 1)
            return;

        time += Time.deltaTime;
        if (time > spawnTimer)
        {
            SpawnMonster();
            time = 0;
        }
    }

    public void SpawnMonster()
    {
        Instantiate(monster, transform);
    }
}
