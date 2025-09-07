using UnityEngine;

public class MainCameraFollower : MonoBehaviour
{
    [SerializeField] Camera baseCam;

    private void Update()
    {
        transform.position = Camera.main.transform.position;
    }
}
