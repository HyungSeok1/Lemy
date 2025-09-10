using UnityEngine;

public class NPCDestroyer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("NPC"))
        {
            Destroy(collision.gameObject);
        }
    }
}
