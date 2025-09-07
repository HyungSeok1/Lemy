using UnityEngine;

public class OctileryCannon : MonoBehaviour
{
    public Transform target;

    void Update()
    {
        Vector3 dir = target.position - transform.position;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90 );
    }
}
