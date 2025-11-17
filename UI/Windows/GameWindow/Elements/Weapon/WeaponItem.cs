using Swat.Game.Entities.Weapons.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Swat.Game.UI.Windows.GameWindow.Elements.Weapon
{
	public class WeaponItem : MonoBehaviour
	{
		[field: SerializeField] public HudWeaponAnimation Animation { get; private set; }
		[field: SerializeField] public Image Icon { get; private set; }
		[field: SerializeField] public Image BulletIcon { get; private set; }
		[field: SerializeField] public Image AmmoProgressBar { get; private set; }
		[field: SerializeField] public TMP_Text AmmoCountText { get; private set; }

		public WeaponType WeaponType { get; private set; }

		public void Setup(WeaponType weaponType, Sprite weaponSprite, Sprite bulletSprite)
		{
			WeaponType = weaponType;
			Icon.sprite = weaponSprite;
			BulletIcon.sprite = bulletSprite;
		}
		
		public void SetActive(bool active)
		{
			gameObject.SetActive(active);
		}
	}
}