using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 좌우 원근감을 주기 위해 존재하는 parallex 기능을 2차원으로 확장한 로직의 클래스입니다.
/// 
/// 배경 GameObject는 직접 생성하는 것이 아닌, parallexParent 아래에 붙어있는 객체가 할당되어 사용됩니다.
/// 
/// </summary>
/// 
[ExecuteInEditMode]
public class ParallaxBackground : MonoBehaviour
{
    public ParallaxCamera parallaxCamera; //연결안돼있으면 Start()에서 자동연결됨 
    List<ParallaxLayer> parallaxLayers = new List<ParallaxLayer>();

    [SerializeField] private int tileCountX = 3;
    [SerializeField] private int tileCountY = 3;

    void Start()
    {
        if (parallaxCamera == null)
            parallaxCamera = Camera.main.GetComponent<ParallaxCamera>();

        SetLayers();
    }

    private void OnEnable()
    {
        parallaxCamera.onCameraTranslateAction += Move;
    }

    private void OnDisable()
    {
            parallaxCamera.onCameraTranslateAction -= Move;
    }

    void Move(Vector2 delta)
    {
        foreach (ParallaxLayer layer in parallaxLayers)
            layer.Move(delta);

        // 2) 각 레이어별로 래핑 검사
        foreach (ParallaxLayer layer in parallaxLayers)
            WrapLayerTiles(layer.transform);
    }

    // 첫 세팅
    void SetLayers()
    {
        parallaxLayers.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            ParallaxLayer layer = transform.GetChild(i).GetComponent<ParallaxLayer>();

            if (layer != null)
            {
                parallaxLayers.Add(layer);
                layer.name = "Layer-" + i;
            }
        }


        // 2) 타일 하나의 크기
        //    (여기서는 첫 번째 레이어의 Sprite 크기를 사용)
        var firstSR = parallaxLayers[0].GetComponent<SpriteRenderer>();
        float tileW = firstSR.bounds.size.x;
        float tileH = firstSR.bounds.size.y;

        // 3) 격자 중앙을 (0,0) 으로 맞추기 위한 오프셋
        float startX = -((tileCountX - 1) * tileW) / 2f;
        float startY = -((tileCountY - 1) * tileH) / 2f;

        // 4) 각 레이어 인덱스별로 (col, row) 계산하고 위치 지정
        for (int i = 0; i < parallaxLayers.Count; i++)
        {
            int col = i % tileCountX;
            int row = i / tileCountX;

            float x = startX + (col * tileW);
            float y = startY + (row * tileH);

            var layerT = parallaxLayers[i].transform;
            layerT.localPosition = new Vector3(x, y, layerT.localPosition.z);
            layerT.name = $"Layer-{col},{row}";
        }
    }
    // 무한 Parallex가 되도록 배경 Layer 스프라이트 이동시켜주는 함수
    void WrapLayerTiles(Transform t)
    {
        var sr = t.GetComponent<SpriteRenderer>();
        if (sr == null) return;

        float tileW = sr.bounds.size.x;
        float tileH = sr.bounds.size.y;

        int countX = tileCountX;
        int countY = tileCountY;

        // 전체 격자 절반 너비·높이
        float halfGridW = tileW * countX / 2f;
        float halfGridH = tileH * countY / 2f;


        Vector3 camPos = parallaxCamera.transform.position;
        Vector3 dir = t.position - camPos;

        // 가로 
        if (dir.x < -halfGridW)
            t.position += Vector3.right * halfGridW * 2;
        else if (dir.x > halfGridW)
            t.position -= Vector3.right * halfGridW * 2;

        // 세로 
        if (dir.y < -halfGridH)
            t.position += Vector3.up * halfGridH * 2;
        else if (dir.y > halfGridH)
            t.position -= Vector3.up * halfGridH * 2;
    }

}