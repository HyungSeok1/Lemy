using UnityEngine;
using UnityEngine.Playables;

public class ActorControlBehaviour : PlayableBehaviour
{
    // 이동 대상 & 포인트들
    public Transform actor;
    public Transform startPoint;
    public Transform targetPoint;

    // 선택 사항(애니메이션/플립)
    public Animator animator;
    public AnimationClip firstAnimation;
    public AnimationClip secondAnimation;
    public bool flipXAtStart;
    public bool isAnimationLoop;
}