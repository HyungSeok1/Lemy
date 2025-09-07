using System.Collections;
using UnityEngine;

public class EnemyWarning : MonoBehaviour
{
    public float warningTime = 2f;
    [SerializeField] private SpriteRenderer fillRenderer;
    [SerializeField] private Vector3 spawnOffset;

    private Material mat;

    public void StartWarning(Vector3 position, float time)
    {
        transform.position = position;
        warningTime = time;
        gameObject.SetActive(true);

        mat = fillRenderer.material;
        mat.SetFloat("_FillAmount", 0f);

        StartCoroutine(WarningRoutine(position));
    }

    private IEnumerator WarningRoutine(Vector3 spawnPos)
    {
        float elapsed = 0f;
        while (elapsed < warningTime)
        {
            float t = elapsed / warningTime;
            mat.SetFloat("_FillAmount", t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        mat.SetFloat("_FillAmount", 1f);
        Destroy(gameObject);
    }
}
