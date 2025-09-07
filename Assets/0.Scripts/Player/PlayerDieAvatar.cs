using UnityEngine;

public class PlayerDieAvatar : MonoBehaviour
{
    // Animation 이벤트에 할당될 함수
    private void OnDie2End()
    {
        Player.Instance.OnDie2End();
    }
}
