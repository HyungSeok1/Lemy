using UnityEngine;

public class OneEye : MonoBehaviour
{
    public Transform target;
    public float maxDistance;

    Vector3 initialPos;

    void Start()
    {
        initialPos = transform.localPosition;
    }

    void Update()
    {
        Vector3 dir = (target.position - transform.position).normalized;
        transform.localPosition = initialPos + dir * maxDistance;
    }
}
