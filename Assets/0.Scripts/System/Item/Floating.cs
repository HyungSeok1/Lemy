using UnityEngine;

public class Floating : MonoBehaviour
{
    public float amplitude;
    public float frequency;

    Vector3 pos;

    void Start()
    {
        pos = transform.position;
    }

    void Update()
    {
        transform.position = pos + Vector3.up * Mathf.Sin(Time.time * frequency) * amplitude;
    }
}
