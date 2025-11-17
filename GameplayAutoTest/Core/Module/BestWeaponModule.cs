using Swat.Game.Data.Weapon;
using Swat.Game.Entities;
using Swat.Game.Entities.Core.Characters.Player.Controllers;
using Swat.Game.Entities.Core.Common.Models;
using Swat.Game.Entities.Weapons.Core;
using Swat.Game.GameplayRobot.Core.Interfaces;
using Swat.Game.Services.ArsenalService.Core;
using Swat.Game.Services.BattleService;
using UnityEngine;

namespace Swat.Game.GameplayRobot.Core.Module
{
	public class BestWeaponModule : IBestWeaponModule
	{
		private const float EffectiveDistanceOffsetThreshold = 0.5f;
		
		private readonly IBattleService<BattleContext> _battleService;
		private readonly IArsenalService<WeaponStaticData> _arsenalService;

		public BestWeaponModule(
			IBattleService<BattleContext> battleService,
			IArsenalService<WeaponStaticData> arsenalService)
		{
			_battleService = battleService;
			_arsenalService = arsenalService;
		}
		
		public bool CalculateAndSetBestWeapon(ICharacterEntity targetEnemy)
		{
			if (_battleService.PlayerEntity == null)
				return false;

			_battleService.PlayerEntity.CurrentExecutionContext.TryResolveModel(out IItemsModel playerItemsModel);
			_battleService.PlayerEntity.CurrentExecutionContext.TryResolveController(out IPlayerItemsController playerItemsController);

			float distanceToEnemy = (_battleService.PlayerEntity.Transform.position - targetEnemy.Transform.position).magnitude;

			WeaponType takenWeaponType = playerItemsModel.CurrentWeaponType;
			
			var primaryStaticData = _arsenalService.GetWeaponStaticData(_arsenalService.CurrentPrimaryWeapon);
			var secondaryStaticData = _arsenalService.GetWeaponStaticData(_arsenalService.CurrentSecondaryWeapon);

			if (primaryStaticData == null || secondaryStaticData == null)
				return false;
			
			float primaryEffectiveDistanceOffset = CalcEffectiveDistanceOffset(distanceToEnemy, primaryStaticData.MinEffectiveDistance, primaryStaticData.MaxEffectiveDistance);
			float secondaryEffectiveDistanceOffset = CalcEffectiveDistanceOffset(distanceToEnemy, secondaryStaticData.MinEffectiveDistance, secondaryStaticData.MaxEffectiveDistance);

			bool isPrimaryBest = primaryEffectiveDistanceOffset < secondaryEffectiveDistanceOffset;
			bool isSecondaryBest = secondaryEffectiveDistanceOffset < primaryEffectiveDistanceOffset;
			float difference = Mathf.Abs(primaryEffectiveDistanceOffset - secondaryEffectiveDistanceOffset);
			
			if(difference < EffectiveDistanceOffsetThreshold)
				return false;
			
			if (isPrimaryBest)
			{
				if (takenWeaponType == primaryStaticData.WeaponType || _arsenalService.GetAmmoCount(primaryStaticData.WeaponType) == 0)
					return false;

				playerItemsController.OnNoAmmoSwitchToNextWeapon();
				return true;
			}
			
			if (isSecondaryBest)
			{
				if (takenWeaponType == secondaryStaticData.WeaponType || _arsenalService.GetAmmoCount(secondaryStaticData.WeaponType) == 0)
					return false;

				playerItemsController.OnNoAmmoSwitchToNextWeapon();
				return true;
			}

			return false;
		}

		private float CalcEffectiveDistanceOffset(float distanceToTarget, float minEffectiveDistance, float maxEffectiveDistance)
		{
			if(distanceToTarget < maxEffectiveDistance && distanceToTarget > minEffectiveDistance)
				return 0;

			if (distanceToTarget > maxEffectiveDistance)
				return distanceToTarget - maxEffectiveDistance;
			
			if (distanceToTarget < minEffectiveDistance)
				return minEffectiveDistance - distanceToTarget;
			
			return 0;
		}
	}
}