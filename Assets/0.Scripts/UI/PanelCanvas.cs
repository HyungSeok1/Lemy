using UnityEngine;
using UnityEngine.SceneManagement;

public class PanelCanvas : MonoBehaviour
{
    [SerializeField] Canvas canvas;

    private void Start()
    {
        canvas.worldCamera = MainCameraScript.Instance.overlayCamera;
    }
}
