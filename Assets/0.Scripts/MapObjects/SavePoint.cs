using UnityEngine;

/// <summary>
/// 닿으면 저절로 세이브되는 SavePoint입니다.
/// 
/// 위치가 gameObject의 중앙에 저장되도록 되어 있습니다.
/// 
/// </summary>
public class SavePoint : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        SaveSelectPanel.Instance.ActiveSaveloadSlots();
        
    }
}
