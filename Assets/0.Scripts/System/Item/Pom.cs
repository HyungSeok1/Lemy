using UnityEngine;

public class Pom : MonoBehaviour
{
    public int amount;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Player.Instance.health.Heal(amount);
            Destroy(gameObject);
        }
    }
}
