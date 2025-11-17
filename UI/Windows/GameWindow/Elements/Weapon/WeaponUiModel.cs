using System;
using Swat.Game.Services.WindowsService.Core.Common;
using UnityEngine;

namespace Swat.Game.UI.Windows.GameWindow.Elements.Weapon
{
	[Serializable]
	public class WeaponUiModel : IUiModel
	{
		[field: SerializeField] public int fireAmmoAddPerAds { get; private set; }
		[field: SerializeField] public float FireAmmoUnselectTimeSec { get; private set; } = 2f;
		
		public bool IsFireAmmoButtonSelected { get; set; }
		public bool IsAdsIconEnabled { get; set; }
		public bool IsFireAmmoButtonFaded { get; set; } = false;
	}
}