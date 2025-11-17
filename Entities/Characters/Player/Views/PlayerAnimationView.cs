using System;
using Sirenix.OdinInspector;
using Swat.Game.Entities.Characters.Common.Views;
using Swat.Game.Entities.Core.Characters.Player.Views;
using UnityEngine;

namespace Swat.Game.Entities.Characters.Player.Views
{
	[Serializable]
	public class PlayerAnimationView : CharacterAnimationView, IPlayerAnimationView
	{
		[field: Title("General")]
		[field: SerializeField]
		public Transform HandsTransform { get; private set; }

		[field: SerializeField] public Transform HandsTargetTransform { get; private set; }


		[field: Title("Animator Animation Settings")]
		[field: SerializeField]
		public Animator HandsAnimator { get; private set; }

		[field: SerializeField] public string ShootAnimationTriggerName { get; private set; }
		[field: SerializeField] public string WeaponLookAnimationTriggerName { get; private set; }
		[field: SerializeField] public string ReloadAnimationTriggerName { get; private set; }
		[field: SerializeField] public string ReloadCompleteAnimationTriggerName { get; private set; }
		[field: SerializeField] public string RunAnimationParamName { get; private set; }
		[field: SerializeField] public string WieldAnimationTriggerName { get; private set; }
		[field: SerializeField] public string UnWieldAnimationParamName { get; private set; }
		[field: SerializeField] public string NightVisionGogglesAnimationParamName { get; private set; }
	}
}