using UnityEngine;
using UnityEngine.Playables;

public enum DialogueClipType { StartPoint, ContinuePoint }
public class DialogueClip : PlayableAsset
{
    public DialogueClipType clipType;
    public string startDialogueKnotName;

    public bool isEndingPoint;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<DialogueClipBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();

        behaviour.clipType = this.clipType;
        behaviour.isEndingPoint = this.isEndingPoint;

        if (clipType == DialogueClipType.StartPoint)
            behaviour.startDialogueKnotName = this.startDialogueKnotName;

        return playable;
    }
}