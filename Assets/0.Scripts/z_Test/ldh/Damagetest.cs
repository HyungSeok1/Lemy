using UnityEngine;

public class DamageTest : MonoBehaviour
{
    public int damage;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Player.Instance.health.TakeDamage(damage);
    }
}