using System;
using Sirenix.OdinInspector;
using Swat.Game.Entities.Core.Characters.Player.Models;
using UnityEngine;


namespace Swat.Game.Entities.Characters.Player.Models
{
	public class PlayerAutoAimModel : BaseModel, IPlayerAutoAimModel
	{
		[field: Title("AutoAim")]
		[field: SerializeField] public bool IsEnabled { get; set; } = default;
		[field: SerializeField] public string[] LayerMask { get; private set; } = default;
		[field: SerializeField] public string[] IgnoreLayerMask { get; private set; } = default;
	
		[field: Title("TargetPriorityCoefs")]
		[field: SerializeField] public AnimationCurve AngleCoefCurve { get; private set; } 
		[field: SerializeField] public AnimationCurve DistanceCoefCurve { get; private set; } 
	
		[field: Title("RadiusPositionOffset")]
		[field: SerializeField] public AnimationCurve TargetYOffsetCurve { get; private set; } = default;
		
		[field: Title("TargetRadius")]
		[field: SerializeField] public AnimationCurve RadiusCurve { get; private set; }
	
		[field: Title("PointsOnRadius")]
		[field: SerializeField] public AnimationCurve PointsCountCurve { get; private set; } = default;
		[field: SerializeField] public bool ChooseRndPoint { get; private set; } = default;
		[field: SerializeField] public bool IsRndOffset { get; private set; } = default;
		[field: HideIf(nameof(IsRndOffset))]
		[field: SerializeField] public float PointsOffset { get; private set; } = default;

		[field: ShowIf(nameof(IsRndOffset))]
		[field: SerializeField] public bool LeftHasMirrorOffset { get; private set; } = default;

		[field: ShowIf(nameof(IsRndOffset))]
		[field: SerializeField] public float PointsOffsetMin { get; private set; } = default;
		[field: ShowIf(nameof(IsRndOffset))]
		[field: SerializeField] public float PointsOffsetMax { get; private set; } = default;
		
		[field: Title("CoverSettings")]
		[field: SerializeField] public float CoverDistanceZone { get; private set; } = default;
		[field: SerializeField] public float GroundOffsetY { get; private set; } = default;
		
		[field: Title("RotationSensitivity")]
		[field: SerializeField] public bool IsSensEnabled { get; set; } = default;
		[field: SerializeField] public AnimationCurve SqrDistanceToRadiusCurve { get; set; } = default;
		[field: SerializeField] public AnimationCurve RadiusToSensitivityCurve{ get; set; } = default;
		
		[field: Title("Debug")]
		[field: SerializeField] public bool ShouldLogScore { get; set; } = default;

		
		
		public override object Clone()
		{
			return this;
		}
	}
}