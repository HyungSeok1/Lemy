using UnityEngine;

/// <summary>
/// 
/// </summary>
public class SavePoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // UI뜨게해야함
        SaveLoadManager.Instance.SaveGame(SaveLoadManager.Instance.CurrentSlot);
    }
}
