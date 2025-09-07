using UnityEngine;
using UnityEngine.Playables;

public class DialogueClipBehaviour : PlayableBehaviour
{
    public DialogueClipType clipType;
    public string startDialogueKnotName;

    public bool isEndingPoint;


    // Timeline이 처음 이 Behaviour를 시작할 때
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (clipType == DialogueClipType.StartPoint)
            GameEventsManager.Instance.dialogueEvents.EnterDialogue(startDialogueKnotName);
        else if (clipType == DialogueClipType.ContinuePoint)
            DialogueManager.Instance.ContinueStory();
    }

    // Timeline이 이 Behaviour를 끝낼 때
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (isEndingPoint)
            DialogueManager.Instance.ExitDialogue();
    }
}