using UnityEngine;
using System;
using UnityEngine.SceneManagement;

/// <summary>
/// "진짜 카메라는 아니고", 카메라의 이동량을 계산해서 이를 이벤트로 다른 오브젝트에게 알리는 스크립트입니다.
/// 
/// onCameraTranslate에 Move()가 등록되어 있습니다.
/// 
/// </summary>
/// 
[ExecuteInEditMode]
public class ParallaxCamera : MonoBehaviour
{
    public event Action<Vector2> onCameraTranslateAction;

    private Vector3 oldPosition;

    [SerializeField] private int frameThreshold;
    private int frameCount;
    private bool init = false;
    private void Awake()
    {
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            init = false;
            frameCount = 0;
        };
    }

    void LateUpdate()
    {
        if (frameCount <= frameThreshold)
        {
            frameCount++;
            return;
        }
        else if (!init)
        {
            oldPosition = transform.position;
            init = true;
        }

        Vector3 newPosition = transform.position;
        Vector2 delta = new Vector2(newPosition.x - oldPosition.x, newPosition.y - oldPosition.y);

        if (delta != Vector2.zero)
            onCameraTranslateAction?.Invoke(delta);

        oldPosition = newPosition;
    }
}
