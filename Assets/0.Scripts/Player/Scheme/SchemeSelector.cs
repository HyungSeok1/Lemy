using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>
/// 
/// 키보드 숫자 2,3키를 눌러 활성화되는 스킬 선택창 띄우는 기능입니다
/// 
/// Selector의 로직/ UI view 담당하는 클래스
/// 
/// </summary>

public class SchemeSelector : MonoBehaviour
{
    [SerializeField] private GameObject SchemeButtonIconPrefab; // 부모 아래에 '스프라이트용 이미지'가 자식으로 할당된 프리팹 (테두리는 부모에)
    [SerializeField] private float radius;  // 중심으로부터 떨어진 거리
    [SerializeField] private RectTransform uiParent;

    private PlayerSchemeController playerSchemeController;

    private void Start()
    {
       // playerSchemeController = Player.Instance.playerSchemeController;
    }

    // Player Input에서 할당하는 콜백 메서드들
    public void OpenSchemeMenu(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        StartCoroutine(ShowSchemeMenu());
    }

    IEnumerator ShowSchemeMenu()
    {
        int buttonCount = playerSchemeController.learnedSchemes.Count; // 생성할 버튼 총 개수
        float totalAngle = 360f;                       // 한 바퀴
        float angleStep = totalAngle / buttonCount;    // 버튼 사이 각도

        if (buttonCount == 0)
        {
            Debug.LogWarning("배운 스킴이 없음. 메뉴를 표시하지 않음");
            yield break;
        }

        List<IconSchemeButton> buttonsRefList = new(); //인스턴스 담을 List<T>

        for (int i = 0; i < buttonCount; i++)
        {

            // ② 개별 각도 및 라디안 변환
            float angleDeg = angleStep * i;
            float angleRad = angleDeg * Mathf.Deg2Rad;

            // ③ 위치 오프셋 계산 (Cos, Sin 사용) 
            Vector3 offset = new Vector3(
                Mathf.Cos(Mathf.PI / 2 + angleRad) * radius,
                Mathf.Sin(Mathf.PI / 2 + angleRad) * radius,
                0f
            );


            // ④ Instantiate & 부모 지정
            GameObject schemeButtonIconInstance = Instantiate(SchemeButtonIconPrefab, uiParent);
            RectTransform rt = schemeButtonIconInstance.GetComponent<RectTransform>();
            rt.anchoredPosition = offset;

            // learnedSchemes의 IControlScheme 참조를 전달
            var iconSchemeButton = schemeButtonIconInstance.GetComponent<IconSchemeButton>();
            buttonsRefList.Add(iconSchemeButton);
            iconSchemeButton.schemeReference = playerSchemeController.learnedSchemes[i];

            Sprite icon = playerSchemeController.learnedSchemes[i].schemeData.icon;
            schemeButtonIconInstance.transform.Find("IconSpriteImage").GetComponent<Image>().sprite = icon;
        }

        yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.Alpha1));

        foreach (var iconSchemeButton in buttonsRefList)
        {
            if (!iconSchemeButton.isHovered) continue;

            playerSchemeController.ChangeScheme(iconSchemeButton.schemeReference);
            break;
        }

        for (int i = uiParent.childCount - 1; i >= 0; i--)
            Destroy(uiParent.GetChild(i).gameObject);
    }
}
