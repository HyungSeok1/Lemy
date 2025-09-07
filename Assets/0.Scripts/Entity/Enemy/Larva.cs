using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;

public class Larva : Enemy
{
    [SerializeField] private float damage;
    [SerializeField] private ItemData meatItemData;

    Animator anim;


    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    protected override void Start()
    {
        base.Start();
    }


    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        anim.SetTrigger("isHit");
    }

    public override void Knockback(Vector2 direction, float force = 5f)
    {
        base.Knockback(direction, force);
    }



    protected override void Attack(Player player)
    {
        player.health.TakeDamage(damage, (player.transform.position - transform.position).normalized);
    }

    protected override void Die()
    {
        DroppedItemManager.Instance.Drop(meatItemData, transform.position, Vector2.zero);

        base.Die();
    }

    #region FSM

    protected override void OnEnterState(State state)
    {

    }

    protected override void OnUpdateState(State state)
    {

    }

    protected override void OnExitState(State state)
    {

    }


    #endregion

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Player>(out Player player))
            Attack(player);
    }
}
