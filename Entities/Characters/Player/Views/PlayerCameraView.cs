using System;
using Cinemachine;
using Sirenix.OdinInspector;
using Swat.Game.Entities.Core.Characters.Player.Views;
using Swat.Game.Services.WindowsService;
using Swat.Utils;
using UnityEngine;



namespace Swat.Game.Entities.Characters.Player.Views
{
    [Serializable]
    public class PlayerCameraView : IPlayerCameraView
    {
        [field: SerializeField] public Transform PlayerRoot { get; private set; } = default;
        [field: SerializeField] public SerializableDictionary<UiOrientation, CameraOrientationContainer> CameraOrientationContainersMap { get; private set; } = default;
        [field: SerializeField] public CinemachineVirtualCamera FpsVirtualCamera { get; private set; } = default;
        [field: SerializeField] public Transform CameraPointTransform { get; private set; } = default;
        [field: SerializeField] public Transform HeadCameraPointTransform { get; private set; } = default;
        [field: SerializeField] public Transform CameraRootTransform { get; private set; } = default;
        [field: SerializeField] public float SmoothFollowPositionFactor { get; private set; } = default;
        [field: SerializeField] public float SmoothFollowRotationFactor { get; private set; } = default;
        [field: SerializeField] public CinemachineImpulseSource RecoilImpulseSource { get; private set; } = default;
        [field: SerializeField] public CinemachineImpulseSource StepImpulseSource { get; private set; } = default;
        [field: SerializeField] public CinemachineImpulseSource ExplosionImpulseSource { get; private set; } = default;
        [field: SerializeField] public float ScopeDelay { get; private set; } = default;
		[field: SerializeField] public float CameraHeight { get; private set; }

        [field: Title("Gyroscope Settings")]
        [field: SerializeField] public float GyroSmoothing { get; private set; } = 0.1f;
        [field: SerializeField] public float GyroAmount { get; private set; } = 60.0f;
        [field: SerializeField] public float GyroSpeed { get; private set; } = 60.0f;
        [field: SerializeField] public Transform GyroTransform { get; private set; } = default;

		public AnimationCurve StepImpulseSourceCustomShape => StepImpulseSource.m_ImpulseDefinition.m_CustomImpulseShape;
        public Camera MainCamera { get; set; }

        public void Refresh() {}

        
        
        [Serializable]
        public class CameraOrientationContainer
        {
            [field: SerializeField] public CinemachineVirtualCamera MainVirtualCamera { get; private set; } = default;
            [field: SerializeField] public CinemachineVirtualCamera MoveVirtualCamera { get; private set; } = default;
            [field: SerializeField] public CinemachineVirtualCamera ShootVirtualCamera { get; private set; } = default;
        }
    }
}