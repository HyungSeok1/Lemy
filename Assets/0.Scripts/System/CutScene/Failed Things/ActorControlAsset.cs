using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Timeline;


[Serializable]
public class ActorControlAsset : PlayableAsset, ITimelineClipAsset
{
    [HideInInspector] public ExposedReference<Animator> exposedAnimator;
    [HideInInspector] public ExposedReference<Transform> exposedTransform;
    public string actorName;
    public AnimationClip firstAnimation;
    public AnimationClip secondAnimation;
    public bool flipX_AtStart;
    public bool isAnimationLoop;

    public ExposedReference<Transform> targetPoint;
    public ExposedReference<Transform> startPoint;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        return ScriptPlayable<ActorControlBehaviour>.Create(graph);

        var playable = ScriptPlayable<ActorControlBehaviour>.Create(graph);
        var bhv = playable.GetBehaviour();

        var resolver = graph.GetResolver();
        bhv.animator = exposedAnimator.Resolve(resolver);
        bhv.actor = exposedTransform.Resolve(resolver);
        bhv.startPoint = startPoint.Resolve(resolver);
        bhv.targetPoint = targetPoint.Resolve(resolver);
        bhv.firstAnimation = firstAnimation;
        bhv.secondAnimation = secondAnimation;
        bhv.flipXAtStart = flipX_AtStart;
        bhv.isAnimationLoop = isAnimationLoop;

        return playable;
    }

   
    public ClipCaps clipCaps => ClipCaps.ClipIn | ClipCaps.SpeedMultiplier;
}
