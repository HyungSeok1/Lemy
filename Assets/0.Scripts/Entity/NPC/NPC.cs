using UnityEngine;

public class NPC : MonoBehaviour
{
    private Animator animator;
    private bool lastDialogueState = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (animator == null) return;

        bool currentState = DialogueManager.Instance.dialoguePlaying;

        if (currentState != lastDialogueState)
        {
            animator.SetBool("DialoguePlaying", currentState);
            lastDialogueState = currentState;
        }
    }
}
