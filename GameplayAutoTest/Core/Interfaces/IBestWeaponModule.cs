using Swat.Game.Entities;

namespace Swat.Game.GameplayRobot.Core.Interfaces
{
	public interface IBestWeaponModule
	{
		/// <returns>Weapon was changed</returns>
		bool CalculateAndSetBestWeapon(ICharacterEntity targetEnemy);
	}
}