using UnityEngine;
using System.Collections;
using DG.Tweening;

/// <summary>
/// 그냥 GameObject가 회전하도록 하는 코드입니다.
/// 
/// </summary>
public class Spin : ObstacleBase
{
    [SerializeField] private bool reversed;

    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private float _damage;

    protected override float Damage => _damage;
    Vector3 pos;

    public float time = 1.5f;
    private float timer = 0f;
    void Start()
    {
        pos = transform.position;
    }
    void Update()
    {
        transform.Rotate(0f, 0f, (reversed ? -1f : 1f) * rotationSpeed * Time.deltaTime);

        timer += Time.deltaTime;
        if (timer >= time)
        {
            SoundManager.Instance.PlaySFXAt("cogSpin3", pos, 1f);
            Debug.Log("Spin Sound");
            timer = 0f;
            time = Random.Range(1.5f, 3f);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        base.OnTriggerEnter2D(collider);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
    }

}
