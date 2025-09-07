using Ink.Parsed;
using UnityEngine;
using UnityEngine.UI;

public class BrakeBar : MonoBehaviour
{
    PlayerMovement movement;
    Slider slider;
    RectTransform rect;

    Image fill;
    private void Awake()
    {
        slider = GetComponent<Slider>();
        rect = GetComponent<RectTransform>();
        fill = slider.fillRect.GetComponent<Image>();
        
    }
    private void Start()
    {
        movement = Player.Instance.GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        float max = movement.brakeCooldown;
        float lastTime = movement.lastBrakeTime;

        slider.value = (Time.time - lastTime) / max;
        float t = slider.value; // 0~1
        fill.color = Color.Lerp(Color.gray, Color.white, t);
    }

    private void FixedUpdate()
    {
        rect.position = Camera.main.WorldToScreenPoint(Player.Instance.transform.position) + new Vector3(0,-30,0);
    }
}
