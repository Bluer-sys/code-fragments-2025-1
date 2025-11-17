using Swat.Game.Data.Weapon;
using Swat.Game.Entities.Weapons.Core;
using Swat.Game.Services.ArsenalService.Core;
using UnityEngine;

namespace Swat.Game.UI.Windows.GameWindow.Elements.Weapon
{
	public class WeaponSpritesProvider
	{
		private const WeaponType Fallback = WeaponType.MP5K;
		
		private readonly IArsenalService<WeaponStaticData> _arsenalService;
		
		public WeaponSpritesProvider(IArsenalService<WeaponStaticData> arsenalService)
		{
			_arsenalService = arsenalService;
		}

		public bool TryGetWeaponSprite(WeaponType weaponType, out Sprite sprite)
		{
			if (weaponType == WeaponType.None)
			{
				sprite = GetFallbackSprite();
				return false;
			}
			
			var staticData = _arsenalService.GetWeaponStaticData(weaponType);
			if (staticData.InGameWeaponIcon == null)
			{
				sprite = GetFallbackSprite();
				return false;
			}

			sprite = staticData.InGameWeaponIcon;
			return true;
		}

		private Sprite GetFallbackSprite()
		{
			var staticData = _arsenalService.GetWeaponStaticData(Fallback);
			return staticData.InGameWeaponIcon;
		}
	}
}