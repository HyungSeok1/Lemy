using UnityEngine;

public class PanelCanvas : MonoBehaviour
{
    [SerializeField] Canvas canvas;

    private void Start()
    {
        canvas.worldCamera = MainCameraScript.Instance.overlayCamera;
    }
}
