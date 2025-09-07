using UnityEngine;
using UnityEngine.Playables;

public class ActorControlMixerBehaviour : PlayableBehaviour
{
    private bool _cached;
    private Vector3 _defaultPos;

    public override void OnGraphStop(Playable playable)
    {
        // 재생 종료 시 원위치 복귀(원하면 유지해도 됨)
        if (_cached)
        {
            var director = playable.GetGraph().GetResolver() as PlayableDirector;
            if (director != null)
            {
                // 트랙 바인딩 Transform 복원
                foreach (var o in director.playableAsset.outputs)
                {
                    if (o.sourceObject == null) continue;
                    if (o.outputTargetType == typeof(Transform))
                    {
                        var bound = director.GetGenericBinding(o.sourceObject) as Transform;
                        if (bound) bound.position = _defaultPos;
                        break;
                    }
                }
            }
        }
        _cached = false;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var boundActor = playerData as Transform; // 트랙 바인딩된 Transform
        int inputCount = playable.GetInputCount();
        if (inputCount == 0) return;

        // 최초 한 번 기본 위치 캐시
        if (!_cached && boundActor != null)
        {
            _cached = true;
            _defaultPos = boundActor.position;
        }

        // 블렌딩 불필요 → 가장 영향도 높은 클립 하나만 적용
        int activeIndex = -1;
        float maxW = 0f;
        for (int i = 0; i < inputCount; i++)
        {
            float w = playable.GetInputWeight(i);
            if (w > maxW)
            {
                maxW = w;
                activeIndex = i;
            }
        }
        if (activeIndex < 0) return;

        var inputPlayable = (ScriptPlayable<ActorControlBehaviour>)playable.GetInput(activeIndex);
        var data = inputPlayable.GetBehaviour();

        // 우선순위: 바인딩 Transform > 클립에 주입된 actor
        var actor = boundActor != null ? boundActor : data.actor;
        if (actor == null) return;

        // 시작/목표 지점
        Vector3 start = data.startPoint != null ? data.startPoint.position : (_cached ? _defaultPos : actor.position);
        Vector3 end = data.targetPoint != null ? data.targetPoint.position : start;

        double dur = inputPlayable.GetDuration();
        double time = inputPlayable.GetTime();
        float t = (dur > double.Epsilon) ? Mathf.Clamp01((float)(time / dur)) : 1f;

        actor.position = Vector3.Lerp(start, end, t);
    }
}
