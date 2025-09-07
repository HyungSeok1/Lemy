using System.Collections;
using UnityEngine;

public class LaserEmitter : MonoBehaviour
{
    public enum FireMode { Timed, Continuous }

    [Header("References")]
    [SerializeField] private LaserBeam2D beam;
    [Tooltip("발사 기준 Transform(없으면 emitter 자신의 transform.right 사용)")]
    [SerializeField] private Transform fireTransformOverride;

    [Header("Fire Settings")]
    [SerializeField] private FireMode fireMode = FireMode.Timed;
    [SerializeField] private float fireDuration = 2f;  // Timed: 켜지는 시간
    [SerializeField] private float fireInterval = 3f;  // Timed: 쉬는 시간
    [SerializeField] private bool startOnAwake = true;

    private Coroutine fireRoutine;

    private void Awake()
    {
        // LaserBeam에 fireTransform을 넘기고 싶으면 여기에서 주입(선택)
        if (beam && fireTransformOverride)
        {
            var fi = typeof(LaserBeam2D).GetField("fireTransform", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            fi?.SetValue(beam, fireTransformOverride);
        }
    }

    private void OnEnable()
    {
        if (startOnAwake) StartFiring();
    }

    private void OnDisable()
    {
        StopFiring();
        beam?.SetEnabled(false);
    }

    public void StartFiring()
    {
        if (fireRoutine != null) StopCoroutine(fireRoutine);
        fireRoutine = StartCoroutine(FireLoop());
    }

    public void StopFiring()
    {
        if (fireRoutine != null) StopCoroutine(fireRoutine);
        fireRoutine = null;
        beam?.SetEnabled(false);
    }

    private IEnumerator FireLoop()
    {
        switch (fireMode)
        {
            case FireMode.Continuous:
                beam?.SetEnabled(true);
                SoundManager.Instance.PlayLoop("laser1", fireTransformOverride, 0.05f);
                while (enabled) yield return null;
                beam?.SetEnabled(false);
                SoundManager.Instance.StopLoop("laser1");
                yield break;

            case FireMode.Timed:
                while (enabled)
                {
                    // 쿨타임
                    float wait = Mathf.Max(0f, fireInterval);
                    for (float t = 0f; t < wait; t += Time.deltaTime)
                        yield return null;

                    // 발사
                    beam?.SetEnabled(true);
                    float fire = Mathf.Max(0f, fireDuration);
                    SoundManager.Instance.PlayLoop("laser1", fireTransformOverride, 0.05f);
                    for (float t = 0f; t < fire; t += Time.deltaTime)
                        yield return null;
                    SoundManager.Instance.StopLoop("laser1");
                    beam?.SetEnabled(false);
                }
                yield break;
        }
    }
}
