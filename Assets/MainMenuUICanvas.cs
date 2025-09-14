using UnityEngine;

public class MainMenuUICanvas : MonoBehaviour
{
    [SerializeField] private Canvas canvas;

    private void Start()
    {
        canvas.worldCamera = Camera.main;
    }
}
