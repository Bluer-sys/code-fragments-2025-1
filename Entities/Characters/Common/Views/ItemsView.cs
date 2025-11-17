using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Swat.Game.Entities.Characters.AI;
using Swat.Game.Entities.Core.Armor;
using Swat.Game.Entities.Core.Characters.Common.Views;
using Swat.Game.Entities.Weapons.Core;
using Swat.Game.GameControllers.Data.Weapon;
using Swat.Utils;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Swat.Game.Entities.Characters.Common.Views
{
	public class ItemsView : IItemsView
	{
		[field: SerializeField] public Transform WeaponPontTransform { get; private set; }
		[field: SerializeField] public Transform LeftHandPointTransform { get; private set; }
		[field: SerializeField] public string DropWeaponLayerMask { get; private set; }
		[field: SerializeField] public SerializableDictionary<WeaponSlot, Transform> WeaponSlots { get; private set; }

		[field: SerializeField]
		[field: BoxGroup]
		public SerializableDictionary<ArmorType, ArmorPartSettings> ArmorPartSettingsMap { get; private set; }

		[field: SerializeField] public Light HeadLight { get; private set; }

		[field: Title("Night Vision Settings")]
		[field: SerializeField]
		public Light NightVisionLight { get; private set; }

		[field: SerializeField] public Transform NightVisionGoggles { get; private set; }
		[field: SerializeField] public ScriptableRendererFeature NightVisionRendererFeature { get; private set; }
		[field: SerializeField] public Material NightVisionMaterial { get; private set; }
		[field: SerializeField] public string FadeMaterialPropertyName { get; private set; } = "_Fade";
		[field: SerializeField] public string NightVisionEnableMaterialPropertyName { get; private set; } = "_NightVision";

		public IWeaponEntity CurrentWeapon { get; set; }
		public IDictionary<WeaponType, WeaponSpawnContext> Weapons { get; set; } = default;

		public void Refresh()
		{
		}


		[Serializable]
		public class ArmorPartSettings
		{
			[field: SerializeField]
			[field: BoxGroup]
			public Transform ArmorParent { get; private set; }

			[field: SerializeField]
			[field: BoxGroup]
			public Vector3 LocalPosition { get; private set; }

			[field: SerializeField]
			[field: BoxGroup]
			public Vector3 LocalRotation { get; private set; }

			[field: SerializeField]
			[field: BoxGroup]
			public bool IsHelmet { get; private set; }

			[field: SerializeField]
			[field: BoxGroup]
			public Bone Bone { get; private set; }
			
			[field: SerializeField]
			[field: BoxGroup]
			public BoneType BoneType { get; private set; }
		}
	}
}