using RootMotion.Dynamics;
using Swat.Game.Entities.Core.Characters.AI.Models;
using Swat.Utils;
using UnityEngine;

namespace Swat.Game.Entities.Characters.AI.Models
{
	public class AiPhysicalModel : BaseModel, IAiPhysicalModel
	{
		[field: SerializeField] public PuppetMasterHumanoidConfig DefaultConfig { get; private set; }
		[field: SerializeField] public PuppetMasterHumanoidConfig DeathConfig { get; private set; }
		[field: SerializeField] public float HitImpulseMultiplier { get; private set; }
		[field: SerializeField] public SerializableDictionary<BoneType, float> HitImpulseMap { get; private set; }
		[field: SerializeField] public float HitReactionMultiplier { get; private set; }
		[field: SerializeField] public float HitReactionArmorMultiplier { get; private set; }
		[field: SerializeField] public float DeathImpulseMultiplier { get; private set; }
		[field: SerializeField] public float DeathImpulsePelvisMultiplier { get; private set; }
		[field: SerializeField] public AnimationCurve DeathImpulseByDistanceCurve { get; private set; } = AnimationCurve.Linear(0, 1, 1, 1);
		[field: SerializeField] public float StunDuration { get; private set; }
		
		

		public bool IsHitInteractionActive { get; set; }
	}
}