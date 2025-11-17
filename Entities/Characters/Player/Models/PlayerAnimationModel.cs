using Sirenix.OdinInspector;
using Swat.Data.Game;
using Swat.Game.Entities.Core.Characters.Player.Models;
using UnityEngine;

namespace Swat.Game.Entities.Characters.Player.Models
{
	public class PlayerAnimationModel : BaseModel, IPlayerAnimationModel
	{
		[field: SerializeField] public float HideHandsLocalRotateOffset { get; private set; }
		[field: SerializeField] public float HideHandsLocalMoveOffset { get; private set; }
		[field: SerializeField] public float HideHandsDuration { get; private set; }

		[field: Title("Procedure Animation Settings")]
		[field: SerializeField]
		public ArmsProcedureAnimationData DefaultArmsAnimationData { get; private set; }

		[field: SerializeField] public ArmsProcedureAnimationData ScopeArmsAnimationData { get; private set; }

		public bool AnimationLock { get; set; }
		public float CurrentWeaponRunAnimDuration { get; set; }
	}
}