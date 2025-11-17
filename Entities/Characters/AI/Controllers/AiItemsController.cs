using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Swat.Data;
using Swat.Data.Game;
using Swat.Game.Data;
using Swat.Game.Data.AI;
using Swat.Game.Data.Entities.Characters;
using Swat.Game.Data.Weapon;
using Swat.Game.Data.Weapon.Customization;
using Swat.Game.Entities.Bullets;
using Swat.Game.Entities.Characters.Common.Controllers;
using Swat.Game.Entities.Characters.Common.Models;
using Swat.Game.Entities.Characters.Common.Views;
using Swat.Game.Entities.Core.Armor;
using Swat.Game.Entities.Core.Armor.Controllers;
using Swat.Game.Entities.Core.Characters.AI.Controllers;
using Swat.Game.Entities.Core.Characters.AI.Views;
using Swat.Game.Entities.Core.Characters.Common.Controllers;
using Swat.Game.Entities.Core.Characters.Common.Views;
using Swat.Game.Entities.Core.Characters.Player.Models;
using Swat.Game.Entities.Core.Weapons.Controllers;
using Swat.Game.Entities.Weapons.Core;
using Swat.Game.Entities.WeaponCustomizations.Data;
using Swat.Game.GameControllers;
using Swat.Game.GameControllers.Data.Weapon;
using Swat.Game.GameControllers.EntitySpawners.Data;
using Swat.Game.GameControllers.Events.Core;
using Swat.Game.Services.AddressableService.Core;
using Swat.Game.Services.ArsenalService.Core;
using Swat.Game.Services.BattleService;
using Swat.Game.Services.LevelService;
using Swat.Game.Services.LevelService.Core;
using Swat.Game.Services.LevelSystem.Core;
using Swat.Game.Utils;
using Swat.Utils;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Swat.Game.Entities.Characters.AI.Controllers
{
	public class AiItemsController : ItemsController<AiHitInfo>, IAiItemsController
	{
		private readonly IDictionary<ArmorType, IArmorEntity> _armorEntities;
		private readonly IReadOnlyList<IReadOnlyDictionary<ArmorType, IArmorEntity>> _armorLayers;
		private readonly IEnemySpawnContext _spawnContext;
        private readonly IAiLiveView _aiLiveView;
        private readonly IAiAnimationView _aiAnimationView;
		private readonly GameSettingsStaticData _gameSettingsStaticData;
		private readonly IAiPhysicalView _aiPhysicalView;
		private readonly IPlayerEvents _playerEvents;
		private readonly IAddressableService _addressableService;
		private readonly CharactersData _charactersData;

		private IEnemyBehaviourController _enemyBehaviourController;
		private IEnemyAnimationController _aiAnimationController;
		private IPhysicalController<AiHitInfo> _aiPhysicalController;

		private readonly List<IArmorBomberController> _armorBomberControllers = new();

        public bool IsCurrentWeaponPrimary { get; set; }
		public IDictionary<ArmorType, IArmorEntity> ArmorEntities => _armorEntities;
		public IReadOnlyList<IReadOnlyDictionary<ArmorType, IArmorEntity>> ArmorLayers => _armorLayers;
        
		public AiItemsController(ICharacterEntity entity,
			IDictionary<WeaponType, WeaponSpawnContext> weaponSpawnContexts,
			IDictionary<ArmorType, IArmorEntity> armorEntities,
			IReadOnlyList<IReadOnlyDictionary<ArmorType, IArmorEntity>> armorLayers,
			ItemsView itemsView,
			ItemsModel itemsModel,
			SceneEvents events,
			TickableManager tickableManager,
			IEnemySpawnContext spawnContext,
			IAiLiveView aiLiveView,
			IAiAnimationView aiAnimationView,
			IArsenalService<WeaponStaticData> arsenalService,
			ILevelService<StageStaticData, LevelStaticData> levelService,
			IBattleService<BattleContext> battleService,
			GameSettingsStaticData gameSettingsStaticData,
			IAiPhysicalView aiPhysicalView,
			IPlayerEvents playerEvents,
			IAddressableService addressableService,
			CharactersData charactersData)
			: base(entity, weaponSpawnContexts, itemsView, itemsModel, events, tickableManager, aiLiveView, aiAnimationView, arsenalService, battleService, levelService)
		{
			_armorEntities = armorEntities;
			_armorLayers = armorLayers;
			_spawnContext = spawnContext;
            _aiLiveView = aiLiveView;
            _aiAnimationView = aiAnimationView;
            _gameSettingsStaticData = gameSettingsStaticData;
			_aiPhysicalView = aiPhysicalView;
			_playerEvents = playerEvents;
			_addressableService = addressableService;
			_charactersData = charactersData;
		}


		public override void Initialize()
		{
			ShouldHeadlightEnabledOnStart = _spawnContext.SpawnStaticData.IsHeadlightEnabled;
			ShouldFlashlightEnabledOnStart = _spawnContext.SpawnStaticData.IsWeaponFlashlightEnabled;

			base.Initialize();
			
			CharacterEntity.TryGetEntityController(out _enemyBehaviourController);
			CharacterEntity.TryGetEntityController(out _aiAnimationController);
			CharacterEntity.TryGetEntityController(out _aiPhysicalController);

			// Armor Parts Parent
			foreach (var armor in _armorEntities)
			{
				InitializeArmor(armor);
			}
            
			// Armor Layers Parent
			foreach (var layer in _armorLayers)
			{
				foreach (var armor in layer)
				{
					InitializeArmor(armor);
				}
			}
			
			OnWeaponChangedEvent += OnWeaponChanged;
			_aiAnimationView.OnGetWeaponAnimEvent += OnGetWeaponAnimEvent;
			_aiAnimationView.OnMeleeAttackAnimEvent += OnMeleeAttackAnimEvent;
			UpdateWeaponCustomization();
		}

        public override void Deinitialize()
        {
            if (!WasInitialized)
                return;

            base.Deinitialize();

            OnWeaponChangedEvent -= OnWeaponChanged;
            _aiAnimationView.OnGetWeaponAnimEvent -= OnGetWeaponAnimEvent;
            _aiAnimationView.OnMeleeAttackAnimEvent -= OnMeleeAttackAnimEvent;
        }
        
		private void InitializeArmor(KeyValuePair<ArmorType, IArmorEntity> armor)
		{
			if (!ItemsView.ArmorPartSettingsMap.TryGetValue(armor.Key, out ItemsView.ArmorPartSettings armorPartSettings))
			{
				Debug.LogError($"Armor part {armor.Key} is not configured!");
				return;
			}
                
			armor.Value.SetParent(armorPartSettings.ArmorParent);
			armor.Value.SetLocalPosition(armorPartSettings.LocalPosition);
			armor.Value.SetLocalRotation(Quaternion.Euler(armorPartSettings.LocalRotation));

			if (armor.Value.TryGetEntityController(out IArmorPhysicalController armorPhysicalController))
			{
				armorPhysicalController.SetupBone(armorPartSettings.Bone, armorPartSettings.BoneType);
				armorPhysicalController.SetupParent(armorPartSettings.ArmorParent);

				if (armorPartSettings.IsHelmet)
				{
					armorPhysicalController.SetupHelmet(CharacterEntity);
				}
			}
		}

		public override void UseItem(ItemType itemType, bool state = true)
		{
			base.UseItem(itemType, state);

			switch (itemType)
			{
				case ItemType.Bomb:
					foreach (var armorItemsController in _armorBomberControllers)
					{
						if (_enemyBehaviourController.TryGetPositionPoint(out var positionPoint) && positionPoint.IsExplosionPoint)
							armorItemsController.SetExplosionRadius(positionPoint.ExplosionRadius);
						
						armorItemsController.DetonateBombs();
					}
					break;
				
				case ItemType.Knife:
					_aiAnimationController.PlayAnimation(AnimationType.MeleeAttack);
					break;
			}
		}


		public void CheckWeaponForPrimary()
		{
			if (ItemsView.Weapons is not { Count: > 0 })
				return;
            
			WeaponType currentWeaponType = ItemsView.Weapons
													.First(weaponKvP => !weaponKvP.Value.IsRewardWeapon 
																		&& weaponKvP.Value.WeaponEntity == ItemsView.CurrentWeapon).
													Key;

            IsCurrentWeaponPrimary = _spawnContext.SpawnStaticData.PrimaryWeapon == currentWeaponType;
        }

		public void SetArmorHitCollidersEnabled(bool isActive)
		{
			foreach (var pair in _armorEntities)
			{
				pair.Value.TryGetEntityController(out IArmorPhysicalController armorPhysicalController);
				armorPhysicalController.SetArmorColliderEnabled(isActive);
			}

			foreach (var pair in _armorLayers)
			{
				foreach (var armor in pair.Values)
				{
					armor.TryGetEntityController(out IArmorPhysicalController armorPhysicalController);
					armorPhysicalController.SetArmorColliderEnabled(isActive);
				}
			}
		}

		public BoneType GetArmorBone(ArmorType armorType)
		{
			if (!ItemsView.ArmorPartSettingsMap.TryGetValue(armorType, out ItemsView.ArmorPartSettings armorPartSettings))
				return BoneType.None;

			return _aiPhysicalController.GetBoneType(armorPartSettings.ArmorParent);
		}

		public Transform GetArmorParent(ArmorType armorType)
		{
			if (!ItemsView.ArmorPartSettingsMap.TryGetValue(armorType, out ItemsView.ArmorPartSettings armorPartSettings))
				return null;

			return armorPartSettings.ArmorParent;
		}

		public bool HasArmor(Transform armorParent)
		{
			foreach (var kvp in _armorEntities)
			{
				if (!ItemsView.ArmorPartSettingsMap.TryGetValue(kvp.Key, out ItemsView.ArmorPartSettings armorPartSettings))
					continue;
				
				if (armorPartSettings.ArmorParent == armorParent)
					return true;
			}
			
			foreach (var layer in _armorLayers)
			{
				foreach (var kvp in layer)
				{
					if (!ItemsView.ArmorPartSettingsMap.TryGetValue(kvp.Key, out ItemsView.ArmorPartSettings armorPartSettings))
						continue;

					if (armorPartSettings.ArmorParent == armorParent)
						return true;
				}
			}

			return false;
		}


		public void MoveCurrentWeaponToSlot(WeaponSlot slotType)
        {
			if (ItemsView.CurrentWeapon == null)
				return;
			
            ItemsView.CurrentWeapon.Transform.parent = ItemsView.WeaponSlots[slotType];

            ItemsView.CurrentWeapon.Transform.localPosition = Vector3.zero;
            ItemsView.CurrentWeapon.Transform.localRotation = Quaternion.Euler(Vector3.zero);
        }


        public void HideAllWeapons()
        {
            foreach (var weaponSpawnContext in WeaponSpawnContexts.Values)
                weaponSpawnContext.WeaponEntity.SetActive(false);
        }


        public override void DisableArmor()
        {
            base.DisableArmor();

			foreach ((ArmorType _, IArmorEntity entity) in _armorEntities)
			{
				entity.TryGetEntityController(out IArmorLiveController armorLiveController);
                
				armorLiveController?.SetLayerMask(_gameSettingsStaticData.DeadEnemiesLayerIndex);
			}
		}


		protected override void SetupWeaponAmmo(IWeaponEntity weaponEntity)
		{
			base.SetupWeaponAmmo(weaponEntity);
			weaponEntity.TryGetEntityController(out IWeaponShootController weaponShootSystem);

            Dictionary<AmmoType, int> ammoTypes = new Dictionary<AmmoType, int>();
            ammoTypes.TryAdd(_spawnContext.SpawnStaticData.PrimaryAmmoType, int.MaxValue);
            ammoTypes.TryAdd(_spawnContext.SpawnStaticData.SecondaryAmmoType, int.MaxValue);
            weaponShootSystem.SetAmmoTypes(ammoTypes);
        }


		private void OnGetWeaponAnimEvent()
		{
			MoveCurrentWeaponToSlot(WeaponSlot.Arm);
		}

		private void OnMeleeAttackAnimEvent()
		{
			if(_playerEvents.Entity == null)
				return;
			
			_playerEvents.Entity.CurrentExecutionContext.TryResolveController(out ICharacterLiveController<PlayerHitInfo> liveController);
			_playerEvents.Entity.CurrentExecutionContext.TryResolveView(out ICharacterLiveView liveView);
			
			if(!liveView.IsAlive)
				return;
			
			BattleService.GetPrimaryWeaponPower(out float power, out var maxPower );
			
			float playerPower = power;
			float missionPower = BattleService.BattleContext.LevelSettings.TargetWeaponPower;
			float ttkMultiplier = TtkExtensions.GetTtkMultiplier(playerPower, missionPower);
			
			var meleeDamage = _spawnContext.SpawnStaticData.OverrideMeleeDamage ? _spawnContext.SpawnStaticData.MeleeDamage : ItemsModel.KnifeDamage;
			
			liveController.ReceiveHit(new PlayerHitInfo
			{
				Damage = meleeDamage * ttkMultiplier,
				IgnoreImmortality = true
			});
		}
		
		private void OnWeaponChanged()
		{
			UpdateWeaponCustomization();
		}


		private void UpdateWeaponCustomization()
		{
			CheckWeaponForPrimary();

			if (ItemsView.CurrentWeapon == null)
				return;

			ItemsView.CurrentWeapon.TryGetEntityController(out IWeaponCustomizationController customizationSystem);

			IDictionary<WeaponCustomizationSlotType, SerializableReferenceValue<WeaponCustomizationTypeContainer>> weaponCustomizationsMap
				= IsCurrentWeaponPrimary
					? _spawnContext.SpawnStaticData.PrimaryWeaponCustomizations
					: _spawnContext.SpawnStaticData.SecondaryWeaponCustomizations;

            foreach (var weaponCustomizationKvp in weaponCustomizationsMap)
                customizationSystem.SetWeaponCustomization(weaponCustomizationKvp.Key, weaponCustomizationKvp.Value.Value.GetTypeValue(), false);
        }
    }
}
