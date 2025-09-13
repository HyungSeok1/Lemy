using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// skill 선택창 ui
/// </summary>
public class SkillSwapUI : MonoBehaviour
{

    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private GameObject skillSlotPrefab; // 생성할 스킬 슬롯 프리팹

    [SerializeField] private Transform[] skillLineParents; // 6,5,6 스킬 담는 라인
    [SerializeField] private Transform[] selectedSkillSlots;

    private PlayerSkillController playerSkillController;

    private bool skillSwapUIOpened = false;

    private const int SKILLCOUNT = 13;

    public static event Action OnSkillSwapUIOpened;
    public static event Action OnSkillSwapUIClosed;

    private void Start()
    {
        playerSkillController = Player.Instance.playerSkillController;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1) && !skillSwapUIOpened) // GetKeyDown 방식이 괜찮을지...
        {
            skillSwapUIOpened = true;
            canvasGroup.alpha = 1f;
            StartCoroutine(InitializeSkillUI());
        }
        else if(Input.GetKeyDown(KeyCode.Alpha1) && skillSwapUIOpened)
        {
            skillSwapUIOpened = false;
            canvasGroup.alpha = 0f;
           StartCoroutine(DestroySkillUI());
        }

        if (skillSwapUIOpened)
        {
            // TODO : 창 열린동안 캐릭터 못움직이게 하고싶음..
        }
    }

    private IEnumerator InitializeSkillUI()
    {
        // 자식들 삭제
        foreach (Transform child in skillLineParents[0]) GameObject.Destroy(child.gameObject);
        foreach (Transform child in skillLineParents[1]) GameObject.Destroy(child.gameObject);
        foreach (Transform child in skillLineParents[2]) GameObject.Destroy(child.gameObject);

        // skillslot 오브젝트 생성. 6개, 5개마다 다음 라인으로 넘겨줌. 
        for (int i = 0; i < SKILLCOUNT; i++)
        {
            GameObject currentSkillSlotObject;

            if (i < 6)
            {
                currentSkillSlotObject = Instantiate(skillSlotPrefab, skillLineParents[0]);
            }
            else if(i < 6 + 5)
            {
                currentSkillSlotObject = Instantiate(skillSlotPrefab, skillLineParents[1]);
            }
            else if (i < 6 + 5 + 6)
            {
                currentSkillSlotObject = Instantiate(skillSlotPrefab, skillLineParents[2]);
            }
            else //너무 많음
            {
                break;
            }

            if (i < playerSkillController.learnedSkills.Count) // 리스트 갯수 만큼만 할당
            {
                var currentSkillSlot = currentSkillSlotObject.GetComponent<SkillSlot>();
                currentSkillSlot.skill = playerSkillController.learnedSkills[i];

                if (currentSkillSlot.skill.skilldata.icon != null) // icon이 있다면 이미지 할당
                {
                    currentSkillSlot.skillImage.sprite = currentSkillSlot.skill.skilldata.icon;
                }
            }
        }

        yield return LayoutGroupModify(false);

        OnSkillSwapUIOpened?.Invoke();
    }

    private IEnumerator LayoutGroupModify(bool layoutTurnedOn) // 한번 정렬 후 레이아웃 끄기;;
    {
        yield return null; // 한 프레임 대기
        for (int i = 0; i < skillLineParents.Length; i++)
        {
            var layout = skillLineParents[i].GetComponent<HorizontalLayoutGroup>();
            layout.enabled = layoutTurnedOn;
        }
    }

    private IEnumerator DestroySkillUI()
    {
        foreach (Transform lineParent in skillLineParents)
        {
            foreach (Transform child in lineParent)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        yield return LayoutGroupModify(true);

        OnSkillSwapUIClosed?.Invoke();
    }
}
