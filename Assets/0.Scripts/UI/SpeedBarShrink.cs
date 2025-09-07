using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpeedBarShrink : MonoBehaviour
{
    [SerializeField] private Image barImage;
    private float maxSpeed;
    private Player player;

    private void Start()
    {
        player = Player.Instance;
    }

    void Update()
    {
        maxSpeed = player.movement. GetMaxSpeed();
        float curr = (player.movement.speed / maxSpeed) * 0.315f;
        barImage.fillAmount = curr;
    }
}
