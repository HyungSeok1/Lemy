using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// ConfinedCinemachine과 같이 있어야 하는 코드.
/// lock X를 걸면 카메라 X가 고정됨. Y 걸면 Y가 고정.
/// 
/// 
/// </summary>

namespace MyCameraTools
{
    [ExecuteAlways]
    [SaveDuringPlay]
    public class AxisLockExtension : CinemachineExtension
    {
        public bool lockX = false;
        public bool lockY = false;
        private Vector3 lockedPosition;

        protected override void Awake()
        {
            base.Awake();
            lockedPosition = transform.position;
        }

        protected override void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage,
            ref CameraState state,
            float deltaTime)
        {
            if (stage == CinemachineCore.Stage.Body)
            {
                Vector3 pos = state.RawPosition;
                if (lockX) pos.x = lockedPosition.x;
                if (lockY) pos.y = lockedPosition.y;
                state.RawPosition = pos;
            }
        }
    }
}
