using UnityEngine;
using System.Collections;

public class Kung : ObstacleBase
{
    public enum MoveDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    public MoveDirection moveDirection = MoveDirection.Down;
    public float speed;
    public float stopTime;
    public float accel;
    public float maxSpeed;

    float curSpeed;
    bool isStopped = false;

    [SerializeField] private float damage;
    protected override float Damage => damage;

    void Start()
    {
        curSpeed = speed;
    }

    void Update()
    {
        if (isStopped) return;

        curSpeed += accel * Time.deltaTime;
        if (curSpeed > maxSpeed)
            curSpeed = maxSpeed;
            
        switch (moveDirection)
        {
            case MoveDirection.Up:
                transform.position += Vector3.up * curSpeed * Time.deltaTime;
                break;
            case MoveDirection.Down:
                transform.position += Vector3.down * curSpeed * Time.deltaTime;
                break;
            case MoveDirection.Right:
                transform.position += Vector3.right * curSpeed * Time.deltaTime;
                break;
            case MoveDirection.Left:
                transform.position += Vector3.left * curSpeed * Time.deltaTime;
                break;
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        base.OnTriggerEnter2D(collider);
        ChangeDir(collider);
    }

    public void ChangeDir(Collider2D collider)
    {
        curSpeed = speed;
        if (collider.CompareTag("Obstacle") || collider.CompareTag("Wall"))
        {
            SoundManager.Instance.PlaySFX("lightning2", 0.1f);

            StartCoroutine(Stop(stopTime));

            switch (moveDirection)
            {
                case MoveDirection.Up:
                    moveDirection = MoveDirection.Down;
                    break;
                case MoveDirection.Down:
                    moveDirection = MoveDirection.Up;
                    break;
                case MoveDirection.Right:
                    moveDirection = MoveDirection.Left;
                    break;
                case MoveDirection.Left:
                    moveDirection = MoveDirection.Right;
                    break;
            }
        }
    }

    IEnumerator Stop(float stopTime)
    {
        isStopped = true;
        yield return new WaitForSeconds(stopTime);
        isStopped = false;
    }

    

}
