using UnityEngine;

public class Floating : MonoBehaviour
{
    public float amplitude;
    public float frequency;

    public bool isFloating = true;

    Vector3 pos;

    void Start()
    {
        pos = transform.position;
    }

    void Update()
    {
        if (!isFloating) return;
        transform.position = pos + Vector3.up * Mathf.Sin(Time.time * frequency) * amplitude;
    }
}
