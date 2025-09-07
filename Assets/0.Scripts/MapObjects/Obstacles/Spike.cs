using UnityEngine;

public class Spike : ObstacleBase
{
    [SerializeField] private float damage;

    protected override float Damage => damage;

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        base.OnTriggerEnter2D(collider);
    }

}
