using System.Collections;
using UnityEngine;

/// <summary>
/// 
/// Blink Effect for a damaged player.
/// This also make Player INVINCIBLE for 'duration'. 
/// Added 'isInvincible' in Player.cs
/// Added a code line in PlayerHealth.cs
/// 
/// </summary>

public class DamageReaction : MonoBehaviour
{
    [SerializeField] float duration = 2f;
    [SerializeField] float blinkTime = 0.15f;

    [SerializeField] GameObject blinkingObj;
    [SerializeField] bool isBlinking = false;

    public void Knockback(Vector2 direction, float force = 3f)
    {
        if (Player.Instance.rb != null && !Player.Instance.isInvincible)
        {
            Player.Instance.rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
        }
    }

    public void StartBlinking()
    {
        if (!isBlinking)
        {
            StartCoroutine(SelfBlinking(duration, blinkTime));
        }
    }

    IEnumerator SelfBlinking(float duration, float blinkTime)
    {
        if (blinkingObj != null && blinkingObj.TryGetComponent(out SpriteRenderer renderer) && !isBlinking)
        {
            //check if the object is player or not.
            Player player = null;
            if (TryGetComponent(out player)) { player.isInvincible = true; }

            isBlinking = true;

            float t = 0;
            bool isVisible = false;

            while (t <= duration)
            {
                isVisible = !isVisible;
                renderer.color = isVisible ? Color.clear : new Color(1, 1, 1, 0.3f);

                yield return new WaitForSeconds(blinkTime);
                t += blinkTime;
            }

            renderer.color = Color.white;

            isBlinking = false;
            if (TryGetComponent(out player)) { player.isInvincible = false; }
        }
        else
        {
            yield return null;
        }
    }
}
