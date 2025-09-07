using UnityEngine;

[DisallowMultipleComponent]
public class LaserRotator : MonoBehaviour
{
    public enum RotationMode { None, Oscillate, Spin}

    [Header("Mode")]
    [SerializeField] private RotationMode rotationMode = RotationMode.None;

    [Header("Oscillate Settings")]
    [Tooltip("시작 각도(베이스) ±이 값만큼 왕복")]
    [SerializeField] private float oscillateAngle = 45f;     // ±45도
    [SerializeField] private float oscillateSpeed = 120f;    // deg/sec

    [Header("Spin Settings")]
    [SerializeField] private float spinSpeed = 90f;          // deg/sec (양수: 반시계, 음수: 시계)

    private float baseAngleZ;
    private float oscDir = 1f;

    private void Awake()
    {
        baseAngleZ = transform.eulerAngles.z;
    }

    private void Update()
    {
        if (rotationMode == RotationMode.None) return;

        float dt = Time.deltaTime;
        switch (rotationMode)
        {
            case RotationMode.Oscillate:
                {
                    float z = transform.eulerAngles.z + oscillateSpeed * oscDir * dt;
                    float delta = Mathf.DeltaAngle(baseAngleZ, z);
                    // ±oscillateAngle 범위에서 반전
                    if (Mathf.Abs(delta) > Mathf.Abs(oscillateAngle))
                    {
                        oscDir *= -1f;
                        z = baseAngleZ + Mathf.Sign(delta) * Mathf.Abs(oscillateAngle);
                    }
                    transform.rotation = Quaternion.Euler(0f, 0f, z);
                }
                break;
            case RotationMode.Spin:
                {
                    float z = transform.eulerAngles.z + spinSpeed * dt;
                    transform.rotation = Quaternion.Euler(0f, 0f, z);
                }
                break;
        }
    }

    // 런타임에 기준각을 재설정하고 싶을 때 호출(패턴 시작 시점 등)
    public void ResetBaseAngleToCurrent()
    {
        baseAngleZ = transform.eulerAngles.z;
        oscDir = 1f;
    }

    // 인스펙터에서 값 바뀔 때 즉시 적용되도록(편의)
    private void OnValidate()
    {
        if (rotationMode != RotationMode.Oscillate)
        {
            // 왕복 모드가 아니면 왕복 상태 초기화
            oscDir = 1f;
            baseAngleZ = transform.eulerAngles.z;
        }
    }
}
