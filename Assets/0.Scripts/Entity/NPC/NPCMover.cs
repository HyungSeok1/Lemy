using UnityEngine;

public class NPCMover : MonoBehaviour
{
    [SerializeField] float minSpeed;
    [SerializeField] float maxSpeed;
    private float speed;
    private Rigidbody2D rb;
    public Vector3 dir;
    private void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        speed = Random.Range(minSpeed, maxSpeed);

        Vector3 currentPos = this.transform.position;
        currentPos.y += Random.Range(-10f, 10f);
        this.transform.position = currentPos;

    }
    void Update()
    {
        rb.linearVelocity = dir.normalized * speed;
    }
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