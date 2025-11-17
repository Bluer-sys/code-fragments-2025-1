using System.Linq;
using Swat.Game.Data.Weapon;
using Swat.Game.Entities;
using Swat.Game.Entities.Core.Armor;
using Swat.Game.Entities.Core.Characters.AI.Controllers;
using Swat.Game.GameplayRobot.Core.Interfaces;
using Swat.Game.GameplayRobot.Game;
using Swat.Game.Services.ArsenalService.Core;
using Swat.Game.Services.BattleService;
using Swat.Utils;
using UnityEngine;

namespace Swat.Game.GameplayRobot.Core.Module
{
	public class BestBodyPartModule : IBestBodyPartModule
	{
		private readonly IBattleService<BattleContext> _battleService;
		private readonly IArsenalService<WeaponStaticData> _arsenalService;

		public BestBodyPartModule(
			IBattleService<BattleContext> battleService,
			IArsenalService<WeaponStaticData> arsenalService)
		{
			_battleService = battleService;
			_arsenalService = arsenalService;
		}


		public Transform CalculateBestShootTarget(ICharacterEntity enemyTarget, WeaponFormat currentWeaponFormat)
		{
			// By current weapon format
			switch (currentWeaponFormat)
			{
				case WeaponFormat.Pistol:
					return RobotUtils.GetEnemyHead(enemyTarget);

				case WeaponFormat.ShootGun or WeaponFormat.SniperRifle:
					return RobotUtils.GetEnemyPelvis(enemyTarget);
			}

			// By armor
			enemyTarget.CurrentExecutionContext.TryResolveController(out IAiItemsController aiItemsController);
			var armor = aiItemsController.ArmorEntities;
			
			if(armor.Count == 0)
				return RobotUtils.GetEnemyChest(enemyTarget);

			if (armor.Count == 1)
			{
				ArmorType type = armor.First().Key;
				int typeInt = (int)type;

				return typeInt.IsInRange(120, 121) || typeInt.IsInRange(106, 109) || typeInt.IsInRange(114, 116)
					? RobotUtils.GetEnemyPelvis(enemyTarget) 
					: RobotUtils.GetEnemyChest(enemyTarget);
			}

			return RobotUtils.GetEnemyChest(enemyTarget);
		}
	}
}