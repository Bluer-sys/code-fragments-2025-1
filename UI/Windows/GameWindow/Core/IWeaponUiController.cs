using System;
using Swat.Game.Data.Weapon;
using Swat.Game.Entities.Weapons.Core;
using Swat.Game.UI.Windows.GameWindow.Elements.Weapon;

namespace Swat.Game.UI.Windows.GameWindow.Core
{
	public interface IWeaponUiController
	{
		event Action OnChangeWeapon;
		event Action OnReloadWeapon;

		void SetAmmo(int ammoInClip, int clipSize, int ammoCount);

		void RefreshWeapons(WeaponRefreshData data);
		void CreateWeapons();

		void OnCoverStateChanged(bool isInCover);
	}
}