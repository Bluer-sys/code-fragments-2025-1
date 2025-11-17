using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using Swat.Game.Entities.Core.Characters.AI.Views;
using Swat.Utils;
using UnityEngine;

namespace Swat.Game.Entities.Characters.AI.Views
{
	public class AiPhysicalView : IAiPhysicalView
	{
		[field: SerializeField] public Animator PuppetAnimationRoot { get; private set; }
		[field: SerializeField] public Transform EquipRoot { get; private set; }
		[field: SerializeField] public FullBodyBipedIK FullBodyBipedIK { get; private set; }
		[field: SerializeField] public HitReaction HitReaction { get; private set; }
		[field: SerializeField] public Transform[] MuscleTargetsMap { get; private set; }
		[field: SerializeField] public SerializableDictionary<Transform, BoneType> BoneMap { get; private set; }
		[field: SerializeField] public Collider[] HitColliders { get; private set; }
		[field: SerializeField] public Collider WeaponKeepBoneCollider { get; private set; }


		public void Refresh()
		{
		}

		[Button]
		private void CollectHitColliders()
		{
			HitColliders = PuppetAnimationRoot.transform.GetComponentsInChildren<Collider>();
		}
	}
}