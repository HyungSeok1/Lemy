using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private GameObject visualCue;
    [SerializeField] private string dialogueKnotName;
    private bool playerIsNear = false;


    private void Awake()
    {
        playerIsNear = false;
        if (visualCue != null)
        {
            visualCue.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerIsNear && visualCue != null)
        {
            visualCue.SetActive(true);
        }
        else if (visualCue != null)
        {
            visualCue.SetActive(false);
        }
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.inputEvents.onSubmitPressed += SubmitPressed;
    }

    private void OnDisable()
    {
        if (GameEventsManager.Instance is not null)
            GameEventsManager.Instance.inputEvents.onSubmitPressed -= SubmitPressed;
    }

    /// <summary>
    /// 대화 진입용
    /// </summary>
    /// <param name="inputEventContext"></param>
    private void SubmitPressed(InputEventContext inputEventContext)
    {
        if (CutsceneManager.Instance.isCutscenePlaying) return;
        if (!playerIsNear || inputEventContext.Equals(InputEventContext.DIALOGUE) || dialogueKnotName.Equals("")) return;
        // 거리조건, InputEventContext DEFAULT임 조건, "" (Quest인것) 조건 세개만족시
        GameEventsManager.Instance.dialogueEvents.EnterDialogue(dialogueKnotName);
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.CompareTag("Player"))
        {
            playerIsNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D otherCollider)
    {
        if (otherCollider.CompareTag("Player"))
        {
            playerIsNear = false;
        }
    }
}
