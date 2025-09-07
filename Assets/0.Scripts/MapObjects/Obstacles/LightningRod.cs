using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningRod : MonoBehaviour
{
    [SerializeField] private GameObject lightningPrefab;
    [SerializeField] private GameObject lightningWarnerPrefab;

    public float lightningInterval = 4f;
    public float warningDuration = 2f;
    public float lightningLength = 50f; //번개의 길이

    private List<float> possibleAngles = new List<float>();

    private bool isActive = true;

    void Start()
    {
        for (int i = 0; i < 24; i++) // 0도부터 345도까지 15도 간격
            possibleAngles.Add(i * 15f);

        if (isActive) StartCoroutine(SpawnLightningRoutine());
    }

    public void SetActive(bool active)
    {
        isActive = active;
        if (isActive)
        {
            if (lightningWarnerPrefab == null || lightningPrefab == null) return;
            StartCoroutine(SpawnLightningRoutine());
        }
        else
        {
            StopAllCoroutines();
        }
    }

    IEnumerator SpawnLightningRoutine()
    {
        while (isActive)
        {
            List<float> selectedAngles = new List<float>();
            while (selectedAngles.Count < 3)
            {
                float randomAngle = possibleAngles[Random.Range(0, possibleAngles.Count)];
                if (!selectedAngles.Contains(randomAngle))
                    selectedAngles.Add(randomAngle);
            }

            foreach (float angle in selectedAngles)
            {
                StartCoroutine(SpawnLightningAtAngle(angle));
            }
            yield return new WaitForSeconds(lightningInterval);
        }
    }

    IEnumerator SpawnLightningAtAngle(float angleDeg)
    {
        Quaternion rot = Quaternion.Euler(0, 0, angleDeg);
        Vector2 direction = rot * Vector2.up;
        Vector2 spawnPosition = (Vector2)transform.position + direction * (lightningLength / 2f);
        GameObject warning = Instantiate(lightningWarnerPrefab, spawnPosition, Quaternion.identity);
        warning.transform.rotation = Quaternion.Euler(0, 0, angleDeg);
        // warning.transform.localScale = new Vector3(2, lightningLength, 1);

        Destroy(warning, warningDuration); // Destroy the warning after the duration
        StartCoroutine(BlinkWarning(warning, warningDuration));
        yield return new WaitForSeconds(warningDuration);

        GameObject lightning = Instantiate(lightningPrefab, spawnPosition, Quaternion.identity);
        lightning.transform.rotation = Quaternion.Euler(0, 0, angleDeg);
        // lightning.transform.localScale = new Vector3(2, lightningLength, 1);
        SoundManager.Instance.PlaySFX("lightning3", 0.1f);
    }

    IEnumerator BlinkWarning(GameObject warning, float duration, float blinkInterval = 0.2f)
    {
        SpriteRenderer sr = warning.GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        float timer = 0f;
        bool visible = true;

        while (timer < duration)
        {
            visible = !visible;
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, visible ? 1f : 0f);
            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }
    }

}
