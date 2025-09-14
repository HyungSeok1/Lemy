using UnityEngine;

public class PlayerDieAvatar : MonoBehaviour
{
    public void DisableSelf()
    {
        gameObject.SetActive(false);
    }
}
