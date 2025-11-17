using System;
using System.Collections.Generic;
using System.Linq;
using Swat.Game.Data.Weapon;
using Swat.Game.Entities.Weapons.Core;
using Swat.Game.Services.WindowsService;
using Swat.Game.UI.Windows.GameWindow.Elements.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Swat.Game.UI.Windows.GameWindow.Elements.Weapon
{
	public class WeaponPanel : BaseWindow
	{
		public event Action OnChangeWeaponButtonClicked;
		public event Action OnAddFireAmmoClicked;
		public event Action OnReloadButtonClicked;


		[SerializeField] private Button changeWeaponButton;
		[SerializeField] private Button reloadButton;
		[SerializeField] private WeaponItem[] weaponItems;
		[SerializeField] private Sprite pistolBullet;
		[SerializeField] private Sprite rifleBullet;

		[Header("Fire Ammo Booster")] 
		[SerializeField] private Button fireAmmoButton;
		[SerializeField] private TMP_Text fireAmmoCountText;
		[SerializeField] private CanvasGroup fireAmmoCanvasGroup;
		[SerializeField] private HudBonusAnimation ammoBonusAnimation;
		[SerializeField] private GameObject rewardedVisualRoot;


		private Dictionary<WeaponType, WeaponItem> _weaponItemsMap;
		private BoosterCoverFadeAnimation _boosterCoverFadeAnimation;
		
		
		public void RefreshWeapons(WeaponRefreshData data, WeaponSpritesProvider weaponSpritesProvider)
		{
			if (!IsValidData(data))
				return;

			_weaponItemsMap ??= new();
			
			InitializeWeapons(data, weaponSpritesProvider);
			RefreshChangeWeaponButtonInteractable(data);

			if (_weaponItemsMap.TryGetValue(data.ActiveWeaponType, out var activeItemData))
			{
				activeItemData.Animation.ScheduleAnimation(HudWeaponAnimation.HudWeapon.Select);
			}

			return;
			if (data.InactiveWeaponType == data.ActiveWeaponType)
				return;
			
			if (_weaponItemsMap.TryGetValue(data.InactiveWeaponType, out var inactiveItemData))
			{
				inactiveItemData.Animation.ScheduleAnimation(HudWeaponAnimation.HudWeapon.NotSelected);
			}
		}
		
		public void RefreshAmmoText(WeaponType weapon, int ammoInClip, int clipSize, int ammoCount)
		{
			if (_lastAmmoInClip ==  ammoInClip && _lastClipSize == clipSize && _lastAmmoCount == ammoCount)
				return;
			
			if (_weaponItemsMap != null && _weaponItemsMap.TryGetValue(weapon, out var data))
			{
				data.AmmoProgressBar.fillAmount = ammoInClip / (float)clipSize;
				data.AmmoCountText.text = $"{ammoInClip}/{ammoCount}";
			}
			
			_lastAmmoInClip = ammoInClip;
			_lastClipSize = clipSize;
			_lastAmmoCount = ammoCount;
		}


		public void SetFireAmmoButtonFaded(bool isFaded, bool instant)
		{
			_boosterCoverFadeAnimation ??= new BoosterCoverFadeAnimation(fireAmmoCanvasGroup);
			
			fireAmmoCanvasGroup.blocksRaycasts = !isFaded;
			
			if (isFaded)
				_boosterCoverFadeAnimation.FadeOut(instant);
			else
				_boosterCoverFadeAnimation.FadeIn(instant);
		}


		public void RefreshFireAmmoButton(bool isSelected, bool hasAmmo, bool isAdsIconEnabled)
		{
			if (hasAmmo)
			{
				ammoBonusAnimation.ScheduleAnimation(isSelected 
					? HudBonusAnimation.HudBonusState.ShowSelected
					: HudBonusAnimation.HudBonusState.HideSelected);
			}
			else
			{
				ammoBonusAnimation.ScheduleAnimation(isSelected 
					? HudBonusAnimation.HudBonusState.ShowNotBonusSelect
					: HudBonusAnimation.HudBonusState.HideNotBonusNotSelect);
			}
			
			rewardedVisualRoot.SetActive(isSelected && isAdsIconEnabled);
		}
		

		public void RefreshFireAmmoCount(int count)
		{
			fireAmmoCountText.text = count.ToString();
		}


		public void CreateWeapons(WeaponRefreshData data, WeaponSpritesProvider weaponSpritesProvider)
		{
			InitializeWeapons(data, weaponSpritesProvider);
		}


		private void InitializeWeapons(WeaponRefreshData data, WeaponSpritesProvider weaponSpritesProvider)
		{
			if (!IsValidData(data))
				return;
			
			if (IsWeaponsMapUpToDate(data))
				return;
			
			_weaponItemsMap.Clear();
			
			bool isInactiveDefined = IsInactiveDefined(data);
			var firstWeaponItem = weaponItems[0];
			var secondWeaponItem = weaponItems[1];
			
			InitializeWeaponItem(firstWeaponItem, data.ActiveWeapon, data.ActiveWeaponType, weaponSpritesProvider);
			_weaponItemsMap.Add(data.ActiveWeaponType, firstWeaponItem);
			
			if (isInactiveDefined)
			{
				InitializeWeaponItem(secondWeaponItem, data.InactiveWeapon, data.InactiveWeaponType, weaponSpritesProvider);
				_weaponItemsMap.Add(data.InactiveWeaponType, secondWeaponItem);
			}
			else
			{
				secondWeaponItem.SetActive(false);
			}
		}

		private bool IsValidData(WeaponRefreshData data)
		{
			return data.ActiveWeaponType != WeaponType.None &&
			data.ActiveWeapon != WeaponFormat.None;
		}


		private void RefreshChangeWeaponButtonInteractable(WeaponRefreshData data)
		{
			bool isInactiveDefined = IsInactiveDefined(data);
			changeWeaponButton.interactable = isInactiveDefined;
		}

		


		private void InitializeWeaponItem(WeaponItem item, WeaponFormat format, WeaponType type, WeaponSpritesProvider weaponSpritesProvider)
		{
			if (!weaponSpritesProvider.TryGetWeaponSprite(type, out var sprite))
			{
				Debug.LogWarning($"Sprite for weapon {type} not found");
			}
			
			var bulletSprite = format == WeaponFormat.Pistol ? pistolBullet : rifleBullet;
			item.Setup(type, sprite, bulletSprite);
			item.SetActive(true);
		}
		

		private bool IsWeaponsMapUpToDate(WeaponRefreshData data)
		{
			_weaponItemsMap ??= new Dictionary<WeaponType, WeaponItem>();
			var isInactiveDefined = IsInactiveDefined(data);
			var mapSize = isInactiveDefined ? 2 : 1;

			if (_weaponItemsMap.Count != mapSize)
				return false;

			if (!_weaponItemsMap.ContainsKey(data.ActiveWeaponType))
				return false;

			if (isInactiveDefined && !_weaponItemsMap.ContainsKey(data.InactiveWeaponType))
				return false;

			if (_weaponItemsMap.Values.All(i => i.WeaponType != data.InactiveWeaponType) ||
				_weaponItemsMap.Values.All(i => i.WeaponType != data.ActiveWeaponType))
				return false;
			
			return true;
		}

		private bool IsInactiveDefined(WeaponRefreshData data)
		{
			WeaponFormat inactiveWeapon = data.InactiveWeapon;
			WeaponFormat activeWeapon = data.ActiveWeapon;
			
			bool isInactiveDefined = inactiveWeapon != WeaponFormat.None
									 && activeWeapon != WeaponFormat.None
									 && activeWeapon != WeaponFormat.Stationary
									 && data.InactiveWeaponType != data.ActiveWeaponType
									 && false;

			return isInactiveDefined;
		}

		
		private void Start()
		{
			changeWeaponButton.onClick.AddListener(() => OnChangeWeaponButtonClicked?.Invoke());
			fireAmmoButton.onClick.AddListener(() => OnAddFireAmmoClicked?.Invoke());
			reloadButton.onClick.AddListener(() => OnReloadButtonClicked?.Invoke());
		}
	}
}
