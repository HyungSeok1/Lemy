using UnityEngine;

/// <summary>
/// 씬 간 전환하는 포탈입니다.
/// 
/// 챕터, 스테이지 정보를 가지고 있으며
/// 
/// "exitID-entranceID"로 출구/입구가 서로 연결됩니다.
/// 
/// </summary>
public class PortalEntrance : MonoBehaviour
{
    [SerializeField] private int targetChapter;
    [SerializeField] private int targetMap;
    [SerializeField] private int targetNumber;
    [SerializeField] private string entranceID;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Player>(out _))
        {
            string sceneName = $"Scene{targetChapter}_{targetMap}_{targetNumber}";
            GameStateManager.Instance.UpdateStateData(new StateData(targetChapter, targetMap, targetNumber));
            SceneTransitionManager.Instance.StartPortalTransition(sceneName, entranceID);
        }
    }
}
