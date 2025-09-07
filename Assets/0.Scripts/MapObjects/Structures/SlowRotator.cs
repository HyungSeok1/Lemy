using UnityEngine;

/// <summary>
/// 그냥 회전..
/// 
/// </summary>
public class SlowRotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;

    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}
