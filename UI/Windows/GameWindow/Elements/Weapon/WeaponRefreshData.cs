using Swat.Game.Data.Weapon;
using Swat.Game.Entities.Weapons.Core;

namespace Swat.Game.UI.Windows.GameWindow.Elements.Weapon
{
	public struct WeaponRefreshData
	{
		public WeaponFormat ActiveWeapon { get; set; }
		public WeaponFormat InactiveWeapon { get; set; }
		public WeaponType ActiveWeaponType { get; set; }
		public WeaponType InactiveWeaponType { get; set; }
	}
}