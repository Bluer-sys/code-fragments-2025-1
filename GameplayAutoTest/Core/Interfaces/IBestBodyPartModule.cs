using Swat.Game.Data.Weapon;
using Swat.Game.Entities;
using UnityEngine;

namespace Swat.Game.GameplayRobot.Core.Interfaces
{
	public interface IBestBodyPartModule
	{
		Transform CalculateBestShootTarget(ICharacterEntity enemyTarget, WeaponFormat currentWeaponFormat);
	}
}