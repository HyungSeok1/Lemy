using UnityEngine;

public class LdhTest : MonoBehaviour
{
    public int damage;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Player.Instance.health.TakeDamage(damage);
        }

        

    }

}
