using Swat.Game.Entities.Characters.Player.Views;
using Swat.Game.Entities.Core.Characters.AI.Models;
using UnityEngine;

namespace Swat.Game.Entities.Characters.AI.Models
{
	public class AiAgentMovementModel : BaseModel, IAiAgentMovementModel
	{
		[Header("Rotation")]
		[field: SerializeField]
		public float RotationAngleOffset { get; private set; }

		[field: SerializeField] public float MinRotationAngle { get; private set; }
		[field: SerializeField] public float RotationDuration { get; private set; }

		[Header("Movement")]
		[field: SerializeField] public float CrouchArrivedPositionDistanceOffset { get; private set; } = 1.0f;
		[field: SerializeField] public float EscapeArrivedPositionDistanceOffset { get; private set; } = 2f;
		[field: SerializeField] public float JumpPower { get; private set; } = 1.0f;
		[field: SerializeField] public float JumpDuration { get; private set; } = 1.0f;
		[field: SerializeField] public AnimationCurve JumpCurve { get; private set; }

		[field: SerializeField] public float HitSpeedMultiplier { get; private set; } = 1.0f;
		[field: SerializeField] public float HitSpeedDecreaseTime { get; private set; } = 1.0f;
		[field: SerializeField] public float SittingSpeedMultiplier { get; private set; } = 1.0f;
		[field: SerializeField] public AnimationCurve RealToAngularSpeedCurve { get; private set; }
		
		public bool IsRotating { get; set; }
		public CoverStatus LastCoverStatus { get; set; }
	}
}