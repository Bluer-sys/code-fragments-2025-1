using Sirenix.OdinInspector;
using Swat.Game.Data.Weapon;
using Swat.Game.Entities.Characters.AI;
using Swat.Game.Entities.Core.Characters.Common.Models;
using Swat.Game.Entities.Core.Common.Models;
using Swat.Game.Entities.Weapons.Core;
using Swat.Game.Services.AudioService;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Swat.Game.Entities.Characters.Common.Models
{
	public class ItemsModel : BaseModel, IItemsModel, IFlashLightModel
	{
		[field: Title("Weapon Settings")]
		[field: SerializeField]
		public float WeaponActivationDelay { get; private set; }

		[field: SerializeField] public bool IsWeaponLayerMaskChangeRequired { get; private set; }
		[field: SerializeField] public bool IsWeaponWieldSoundsActive { get; private set; }
		[field: SerializeField] public Vector2 DropWeaponImpulseMultiplierMinMax { get; private set; }
		[field: SerializeField] public Vector2 DropWeaponTorqueImpulseMultiplierMinMax { get; private set; }
		[field: SerializeField] public float NoAmmoSwitchWeaponDelay { get; private set; }
		
		[field: Tooltip("Delay before weapon turns down after weapon switch in cover.")]
		[field: SerializeField] public float DelayBeforeWeaponTurnDown { get; private set; }
		
		[field: Tooltip("When weapon is inactive, it will be auto-reloaded after being inactive for some time.")]
		[field: SerializeField] public float DelayBeforeReloadInactiveWeapon { get; private set; }
		
		[field: Title("Headlight Settings")]
		[field: SerializeField]
		public float DimHeadLightIntensity { get; private set; }

		[field: SerializeField] public float DimHeadLightDuration { get; private set; }
		[field: SerializeField] public float LitHeadLightIntensity { get; private set; }
		[field: SerializeField] public float LitHeadLightDuration { get; private set; }

		[field: Title("Flashlight Settings")]
		[field: SerializeField]
		public float FlashlightMaxIntensity { get; private set; }

		[field: SerializeField] public float FlashlightLowIntensity { get; private set; }
		[field: SerializeField] public float FlashlightEnableFactor { get; private set; }
		[field: SerializeField] public float FlashlightDisableFactor { get; private set; }

		[field: Title("Night Vision Settings")]
		[field: SerializeField]
		public float NightVisionEnableDuration { get; private set; }

		[field: SerializeField] public Vector3 NightVisionHandsPosition { get; private set; }
		[field: SerializeField] public SoundId NightVisionSoundID { get; private set; }
		[field: SerializeField] public float NightVisionDefaultIntensity { get; private set; }
		[field: SerializeField] public float NightVisionFadeDelay { get; private set; } = 1;
		[field: SerializeField] public float NightVisionFadeInDuration { get; private set; } = 1;
		[field: SerializeField] public float NightVisionFadeOutDuration { get; private set; } = 1;
		[field: SerializeField] public AnimationCurve NightVisionFadeOutCurve { get; private set; }
		[field: SerializeField] public bool ShouldNightVisionEnabledOnStart { get; private set; }

		[field: Title("Melee Settings")]
		[field: SerializeField] public float KnifeDamage { get; private set; }
		
		public WeaponFormat CurrentWeaponFormat { get; set; }
		public WeaponType CurrentWeaponType { get; set; }
		public bool HasCurrentWeaponOpticalScope { get; set; }
		public bool IsNightVisionEnabled { get; set; }
		public bool IsHeadlightEnabled { get; set; }
		public bool HasRewardWeapon { get; set; }
		public bool IsWeaponChangeProcessActive { get; set; }
		public StrapperRope StrapperRope { get; set; }
		public WeaponStaticData CurrentWeaponStaticData { get; set; }
	}
}