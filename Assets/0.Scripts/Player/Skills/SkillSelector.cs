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

/// <summary>
///  구버전 스킬 UI
/// </summary>
public class SkillSelector : MonoBehaviour
{
    [SerializeField] private PlayerSkillController playerSkillController;

    [SerializeField] private GameObject skillButtonIconPrefab; // 부모 아래에 '스프라이트용 이미지'가 자식으로 할당된 프리팹 (테두리는 부모에)
    [SerializeField] private float radius;  // 중심으로부터 떨어진 거리
    [SerializeField] private RectTransform uiParent;

    // Player Input에서 할당하는 콜백 메서드들
    public void OpenSkillMenu0(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        StartCoroutine(ShowSkillMenu(0));
    }

    // Player Input에서 할당하는 콜백 메서드들
    public void OpenSkillMenu1(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        StartCoroutine(ShowSkillMenu(1));
    }

    IEnumerator ShowSkillMenu(int slot)
    {
        int buttonCount = playerSkillController.learnedSkills.Count; // 생성할 버튼 총 개수
        float totalAngle = 360f;                       // 한 바퀴
        float angleStep = totalAngle / buttonCount;    // 버튼 사이 각도

        List<IconSkillButton> buttonsRefList = new(); //인스턴스 담을 List<T>

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
            GameObject skillButtonIconInstance = Instantiate(skillButtonIconPrefab, uiParent);
            RectTransform rt = skillButtonIconInstance.GetComponent<RectTransform>();
            rt.anchoredPosition = offset;

            // learnedSkill의 Iskill 참조를 전달
            var iconSkillButton = skillButtonIconInstance.GetComponent<IconSkillButton>();
            buttonsRefList.Add(iconSkillButton);
            iconSkillButton.skillReference = playerSkillController.learnedSkills[i];

            Sprite icon = playerSkillController.learnedSkills[i].skilldata.icon;
            skillButtonIconInstance.transform.Find("IconSpriteImage").GetComponent<Image>().sprite = icon;
        }

        if (slot == 0)
            yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.Alpha2));
        else if (slot == 1)
            yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.Alpha3));

        foreach (var iconSkillButton in buttonsRefList)
        {
            if (!iconSkillButton.isHovered) continue;

            if (slot == 0)
            {
                if (playerSkillController.currentSkills[0] != null)
                    playerSkillController.currentSkills[0].ReleaseSkill(); // 기존 스킬 해제
                playerSkillController.ChangeSkill(iconSkillButton.skillReference, 0);
                break;
            }
            else if (slot == 1)
            {
                if (playerSkillController.currentSkills[1] != null)
                    playerSkillController.currentSkills[1].ReleaseSkill(); // 기존 스킬 해제
                playerSkillController.ChangeSkill(iconSkillButton.skillReference, 1);
                break;
            }
        }

        for (int i = uiParent.childCount - 1; i >= 0; i--)
            Destroy(uiParent.GetChild(i).gameObject);
    }
}
