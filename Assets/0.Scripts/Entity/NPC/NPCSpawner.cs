using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [SerializeField] float spawnTime;
    [SerializeField] float speed;
    [SerializeField] Vector3 dir;
    [SerializeField] private GameObject[] NPC;

    float timer;

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

    private GameObject Spawn()
    {
        int ran = Random.Range(0, NPC.Length);
        GameObject spawned = Instantiate(NPC[ran], transform);
        return spawned;
    }
}
