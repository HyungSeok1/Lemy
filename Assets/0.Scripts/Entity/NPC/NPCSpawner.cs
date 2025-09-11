using System.Collections;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [SerializeField] float spawnMinTime;
    [SerializeField] float spawnMaxTime;
    [SerializeField] Vector3 dir;
    [SerializeField] private GameObject[] NPC;
    private bool keepSpawn = true;

    private void Start()
    {
        StartCoroutine(SpawnNPC());
    }
/*
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > spawnTime)
        {
            GameObject spawned = Spawn();
            spawned.GetComponent<Rigidbody2D>().linearVelocity = dir.normalized * speed;
            timer = 0;
        }
    }
*/
    private IEnumerator SpawnNPC()
    {
        while (keepSpawn)
        {
            float newSpawnTime = Random.Range(spawnMinTime, spawnMaxTime);
            int ran = Random.Range(0, NPC.Length);
            GameObject spawned = Instantiate(NPC[ran], transform);
            spawned.GetComponent<NPCMover>().dir = dir;
            yield return new WaitForSeconds(newSpawnTime);
        }
    }

}
