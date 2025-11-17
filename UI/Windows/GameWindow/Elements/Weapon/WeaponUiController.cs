using System;
using Swat.Game.Data.Weapon;
using Swat.Game.Entities;
using Swat.Game.Entities.Core.Characters.Common.Views;
using Swat.Game.Entities.Core.Characters.Player.Views;
using Swat.Game.Entities.Core.Common.Models;
using Swat.Game.Entities.Weapons.Core;
using Swat.Game.Services.AdsService;
using Swat.Game.Services.AdsService.Core;
using Swat.Game.Services.ArsenalService.Core;
using Swat.Game.Services.AudioService;
using Swat.Game.Services.BattleService;
using Swat.Game.Services.GameBoostersService.Core;
using Swat.Game.Services.GameBoostersService.Data;
using Swat.Game.Services.WindowsService;
using Swat.Game.UI.Panels.RewardsAnimatedPanel.Core;
using Swat.Game.UI.Windows.GameWindow.Core;
using Swat.Game.UI.Windows.GameWindow.Elements.Utils;
using Zenject;

namespace Swat.Game.UI.Windows.GameWindow.Elements.Weapon
{
	[Serializable]
	public class WeaponUiController : BaseUiController<WeaponUiModel, WeaponPanel>, IWeaponUiController
	{
		public event Action OnChangeWeapon;
		public event Action OnReloadWeapon;

		private IAudioService<SoundId> _audioService;
		private IBattleService<BattleContext> _battleService;
		private IGameBoostersService _gameBoostersService;
		private IAdsService<AdsPlacementType> _adsService;
		private IArsenalService<WeaponStaticData> _arsenalService;

		private IItemsView _playerItemsView;
		private IItemsModel _playerItemsModel;
		private IPlayerAnimationView _playerAnimationView;
		private IPlayerMovementView _playerMovementView;
		private IRewardsAnimatedPanelUiController _rewardsAnimatedPanel;

		private WeaponRefreshData _lastData = new()
		{
			ActiveWeapon = WeaponFormat.Rifle,
			InactiveWeapon = WeaponFormat.Pistol,
			ActiveWeaponType = WeaponType.None,
			InactiveWeaponType = WeaponType.None
		};

		private BoosterUnselectAnimation _unselectAnimation;
		private WeaponSpritesProvider _spritesProvider;


		[Inject]
		public void Construct(IAudioService<SoundId> audioService,
			IBattleService<BattleContext> battleService,
			IGameBoostersService gameBoostersService,
			IAdsService<AdsPlacementType> adsService,
			IRewardsAnimatedPanelUiController rewardsAnimatedPanel,
			IArsenalService<WeaponStaticData> arsenalService)
		{
			_battleService = battleService;
			_audioService = audioService;
			_gameBoostersService = gameBoostersService;
			_adsService = adsService;
			_rewardsAnimatedPanel = rewardsAnimatedPanel;
			_arsenalService = arsenalService;

			_unselectAnimation = new BoosterUnselectAnimation(UnselectFireAmmo, model.FireAmmoUnselectTimeSec);
			_spritesProvider = new WeaponSpritesProvider(arsenalService);
		}


		public override void RefreshViewBeforeChangeOrientation()
		{
			base.RefreshViewBeforeChangeOrientation();

			view.OnChangeWeaponButtonClicked -= OnChangeWeaponClicked;
			view.OnAddFireAmmoClicked -= ViewOnAddFireAmmoClicked;
			view.OnReloadButtonClicked -= OnReloadClicked;
			_battleService.OnPlayerAuthorized -= BattleServiceOnPlayerAuthorized;
			_battleService.OnPlayerDeAuthorized -= BattleServiceOnPlayerDeAuthorized;
		}


		public override void RefreshViewAfterChangeOrientation()
		{
			base.RefreshViewAfterChangeOrientation();

			view.OnChangeWeaponButtonClicked += OnChangeWeaponClicked;
			view.OnAddFireAmmoClicked += ViewOnAddFireAmmoClicked;
			view.OnReloadButtonClicked += OnReloadClicked;
			_battleService.OnPlayerAuthorized += BattleServiceOnPlayerAuthorized;
			_battleService.OnPlayerDeAuthorized += BattleServiceOnPlayerDeAuthorized;

			RefreshWeapons(_lastData);
			
			SetAmmo(_lastAmmoInClip, _lastClipSize, _lastAmmoCount);
			
			int count = _gameBoostersService.GetBoostersCount(BoosterType.FireAmmo);
			view.RefreshFireAmmoButton(model.IsFireAmmoButtonSelected, count > 0, model.IsAdsIconEnabled);
			view.SetFireAmmoButtonFaded(model.IsFireAmmoButtonFaded, true);
		}


		public void SetAmmo(int ammoInClip, int clipSize, int ammoCount)
		{
			bool skipUiUpdate = _lastAmmoInClip == ammoInClip && _lastClipSize == clipSize && _lastAmmoCount == ammoCount;
			
			_lastAmmoInClip = ammoInClip;
			_lastClipSize = clipSize;
			_lastAmmoCount = ammoCount;

			view.RefreshAmmoText(_lastData.ActiveWeaponType, ammoInClip, clipSize, ammoCount  - ammoInClip + (clipSize - ammoInClip) );
		}


		public void CreateWeapons()
		{
			var data = new WeaponRefreshData();

			if (_arsenalService.CurrentPrimaryWeapon != WeaponType.None)
			{
				var primaryData = _arsenalService.GetWeaponStaticData(_arsenalService.CurrentPrimaryWeapon);
				data.ActiveWeapon = primaryData.WeaponFormat;
				data.ActiveWeaponType = primaryData.WeaponType;
			}
			
			if (_arsenalService.CurrentSecondaryWeapon != WeaponType.None)
			{
				var secondaryData = _arsenalService.GetWeaponStaticData(_arsenalService.CurrentSecondaryWeapon);
				
				if (_arsenalService.CurrentPrimaryWeapon == WeaponType.None)
				{
					data.ActiveWeapon = secondaryData.WeaponFormat;
					data.ActiveWeaponType = secondaryData.WeaponType;
				}
				else
				{
					data.InactiveWeapon = secondaryData.WeaponFormat;
					data.InactiveWeaponType = secondaryData.WeaponType;
				}
			}
			
			view.CreateWeapons(data, _spritesProvider);
			view.RefreshWeapons(data, _spritesProvider);
		}
		

		public void RefreshWeapons(WeaponRefreshData data)
		{
			_lastData = data;
			view.RefreshWeapons(data, _spritesProvider);
		}

		
		public void OnCoverStateChanged(bool isInCover)
		{
			model.IsFireAmmoButtonFaded = !isInCover;
			model.IsFireAmmoButtonSelected = false;
            
			int count = _gameBoostersService.GetBoostersCount(BoosterType.FireAmmo);
            
			view.RefreshFireAmmoButton(model.IsFireAmmoButtonSelected, count > 0, model.IsAdsIconEnabled);
			view.SetFireAmmoButtonFaded(model.IsFireAmmoButtonFaded, false);
		}


		private void OnChangeWeaponClicked()
		{
			_audioService.PlayClickSound();
			
			bool animationPlaying = _playerAnimationView.IsShootAnimationPlaying || 
									_playerAnimationView.IsWieldAnimationPlaying || 
									_playerAnimationView.IsUnWieldAnimationPlaying;

			if (_playerItemsView == null
				|| _playerItemsModel.IsWeaponChangeProcessActive
				|| _playerMovementView.IsMoving
				|| animationPlaying)
				return;

			OnChangeWeapon?.Invoke();
		}
		
		
		private void OnReloadClicked()
		{
			OnReloadWeapon?.Invoke();
		}
	}
}
