using UnityEngine;

public class QuestExampleObject : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Quest Example Complete Triggered");
            GameEventsManager.Instance.miscEvents.QuestExampleComplete();
        }
    }

}
