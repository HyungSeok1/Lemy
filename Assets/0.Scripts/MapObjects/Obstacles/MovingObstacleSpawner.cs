using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class MovingObstacleSpawner : MonoBehaviour
{
    public float spawnTimer;
    public Vector3 targetPosition;
    public GameObject obstacle;
    public float speed;

    List<GameObject> pool;
    GameObject spawned;
    float time = 0;

    void Awake()
    {
        pool = new List<GameObject>();
    }

    public void Update()
    {
        time += Time.deltaTime;
        if (time >= spawnTimer)
        {
            spawned = SpawnObstacle();
            if (spawned)
                MoveObstacle(spawned);
            time = 0;
        }
    }

    public GameObject SpawnObstacle()
    {
        GameObject select = null;
        foreach (GameObject item in pool)
        {
            if (!item.activeSelf)
            {
                select = item;
                select.gameObject.transform.position = transform.position;
                select.SetActive(true);
                break;
            }
        }

        if (select == null)
        {
            select = Instantiate(obstacle, transform);
            select.transform.position = transform.position;
            pool.Add(select);
            Debug.Log("생성");
        }

        return select;
    }

    public void MoveObstacle(GameObject obstacle)
    {
        StartCoroutine(MoveToTarget(obstacle));
    }

    IEnumerator MoveToTarget(GameObject obstacle)
    {
        while (Vector3.Distance(obstacle.transform.position, targetPosition) > 0.01f)
        {
            obstacle.transform.position = Vector3.MoveTowards(
                obstacle.transform.position,
                targetPosition,
                speed * Time.deltaTime
            );
            yield return null;
        }

        obstacle.transform.position = targetPosition; 
        obstacle.SetActive(false);
    }

}
