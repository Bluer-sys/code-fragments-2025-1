using System;
using Swat.Game.Data;
using Swat.Game.Data.Weapon;
using Swat.Game.Entities;
using Swat.Game.Entities.Characters.Player.States;
using Swat.Game.Entities.Core.Characters.Common.Controllers;
using Swat.Game.Entities.Core.Characters.Common.Views;
using Swat.Game.Entities.Core.Characters.Player.Controllers;
using Swat.Game.Entities.Core.Characters.Player.Models;
using Swat.Game.Entities.Core.Characters.Player.Views;
using Swat.Game.Entities.Core.Common.Models;
using Swat.Game.Entities.Core.Environment;
using Swat.Game.Entities.Core.Environment.Views;
using Swat.Game.Entities.States;
using Swat.Game.Entities.Weapons.Core;
using Swat.Game.GameControllers.EntitySpawners.Data;
using Swat.Game.GameControllers.Events.Core;
using Swat.Game.GameplayRobot.Core.Interfaces;
using Swat.Game.Services.ArsenalService.Core;
using Swat.Game.Services.BattleService;
using Swat.Game.Services.CurrencyService.Core;
using Swat.Game.Services.GameBoostersService.Core;
using Swat.Game.Services.GameBoostersService.Data;
using Swat.Game.Services.SaveService;
using Swat.Game.UI.Windows.ArsenalWindow.Core;
using Swat.Game.UI.Windows.DefeatWindow.Core;
using Swat.Game.UI.Windows.GameWindow.Core;
using Swat.Game.UI.Windows.MainMenu.Core;
using Swat.Game.UI.Windows.VictoryWindow.Core;
using Swat.Game.Utils;
using UnityEngine;
using Zenject;

namespace Swat.Game.GameplayRobot.Core
{
	public class GameServiceAdapter : IGameServiceAdapter, IInitializable, IDisposable
	{
		private readonly IBestWeaponModule _bestWeaponModule;
		private readonly IBestBodyPartModule _bestBodyPartModule;
		private readonly IBattleService<BattleContext> _battleService;
		private readonly IPlayerEvents _playerEvents;
		private readonly IDefeatWindowUiController _defeatUiController;
		private readonly IGameBoostersService _gameBoostersService;
		private readonly IArsenalUiController _arsenalUiController;
		private readonly IMainMenuUiController _mainMenuUiController;
		private readonly ILiveUiController _liveUiController;
		private readonly ICurrencyService _currencyService;
		private readonly IArsenalService<WeaponStaticData> _arsenalService;
		private readonly IRaidSaveDataResetController _raidSaveDataResetController;

		private IPlayerStateMachineModel _playerStateMachineModel;
		
		private IPlayerAutoAimController _playerAutoAimController;
		private ICharacterStateMachineBehaviourController _playerStateMachineController;
		private IPlayerMovementController _playerMovementController;
		private IPlayerFootAnimationController _playerFootAnimationController;
		private IPlayerAnimationView _playerAnimationView;
		private ICharacterLiveView _playerLiveView;

		public GameServiceAdapter(IBestWeaponModule bestWeaponModule,
			IBestBodyPartModule bestBodyPartModule,
			IBattleService<BattleContext> battleService, 
			IPlayerEvents playerEvents, 
			IDefeatWindowUiController defeatUiController,
			IGameBoostersService gameBoostersService,
			IArsenalUiController arsenalUiController,
			IMainMenuUiController mainMenuUiController,
			ILiveUiController liveUiController,
			ICurrencyService currencyService,
			IArsenalService<WeaponStaticData> arsenalService,
			IRaidSaveDataResetController raidSaveDataResetController)
		{
			_bestWeaponModule = bestWeaponModule;
			_bestBodyPartModule = bestBodyPartModule;
			_battleService = battleService;
			_playerEvents = playerEvents;
			_defeatUiController = defeatUiController;
			_gameBoostersService = gameBoostersService;
			_arsenalUiController = arsenalUiController;
			_mainMenuUiController = mainMenuUiController;
			_liveUiController = liveUiController;
			_currencyService = currencyService;
			_arsenalService = arsenalService;
			_raidSaveDataResetController = raidSaveDataResetController;
		}

		public void Initialize()
		{
			_battleService.OnPlayerAuthorized += BattleService_OnPlayerAuthorized;
		}

		public void Dispose()
		{
			_battleService.OnPlayerAuthorized -= BattleService_OnPlayerAuthorized;
		}

		private void BattleService_OnPlayerAuthorized(ICharacterEntity playerEntity)
		{
			playerEntity.CurrentExecutionContext.TryResolveModel(out _playerStateMachineModel);
			playerEntity.CurrentExecutionContext.TryResolveView(out _playerAnimationView);
			playerEntity.CurrentExecutionContext.TryResolveView(out _playerLiveView);
			playerEntity.CurrentExecutionContext.TryResolveController(out _playerAutoAimController);
			playerEntity.CurrentExecutionContext.TryResolveController(out _playerStateMachineController);
			playerEntity.CurrentExecutionContext.TryResolveController(out _playerMovementController);
			playerEntity.CurrentExecutionContext.TryResolveController(out _playerFootAnimationController);
		}

		
		public bool HasPlayer => _battleService.PlayerEntity != null;
		public bool IsPlayerMoving => _playerEvents.IsMoving;
		public bool IsPlayerAlive => _playerLiveView.IsAlive;
		public bool HasActiveEnemies => _battleService.ActiveEnemiesAtCurrentGroup != null && _battleService.ActiveEnemiesAtCurrentGroup.Count > 0;
		public bool HasAliveEnemies => _battleService.AliveEnemiesAtCurrentGroup != null && _battleService.AliveEnemiesAtCurrentGroup.Count > 0;
		
		public bool CanShootAtEnemies => HasPlayer 
									   && HasActiveEnemies
									   && !IsPlayerMoving 
									   && !_playerAnimationView.IsReloadAnimationPlaying;

		public bool CanAimAtEnemies => HasPlayer 
									   && HasAliveEnemies 
									   && !IsPlayerMoving 
									   && !_playerAnimationView.IsReloadAnimationPlaying;

		public float PlayerHealthPercent => _playerLiveView.HealthPercent;


		public void ClickReviveOrRetry()
		{
			if (_battleService.IsAllEnemiesDeadInBattle())
			{
				_defeatUiController.ViewOnRetryButtonClicked();
			}
			else
			{
				_gameBoostersService.AddBoosters(BoosterType.MedKit, 1);
				_defeatUiController.ViewOnReviveButtonClicked();
			}
		}

		public void ClickStartLevel()
		{
			_arsenalUiController.OnPlayButtonClicked();
		}

		public void ClickGoToArsenal()
		{
			_mainMenuUiController.GoNext();
		}

		public void ClickUpgradeCurrentWeapon()
		{
			_arsenalUiController.ClickUpgradeCurrentWeapon();
		}

		public void GetRewardAndContinue()
		{
			_currencyService.AddCurrencyValue((CurrencyType.Tickets, 1));

			// ToDo: update for new endgame window
			/*if (_victoryUiController.HasWeaponReward)
				_victoryUiController.RewardedWeaponClick();
			else
				_victoryUiController.RewardedMoneyClick();*/
		}

		
		
		public bool IsEnoughMoneyForCurrentUpgrade()
		{
			return _arsenalUiController.EnoughMoneyForCurrentUpgrade();
		}

		

		public void SetAutoAimLock(bool isLocked)
		{
			_playerAutoAimController?.SetLock(isLocked);
		}

		public void TryUseMedKit()
		{
			if(!HasPlayer || !IsPlayerAlive)
				return;
			
			_gameBoostersService.AddBoosters(BoosterType.MedKit, 1);
			_playerFootAnimationController.PlayHealStanding();
		}
		
		public bool IsHealingProcess()
		{
			return _playerFootAnimationController.IsHealing;
		}

		public bool IsMeaningfulTarget(ICharacterEntity entity)
		{
			return entity != null && SwatUtils.GetEnemyType(entity) == EnemyType.Bomber;
		}

		public ICharacterEntity FindTarget()
		{
			if(_playerAutoAimController == null)
				return null;

			_playerAutoAimController.TryFindBestTarget(out ICharacterEntity bestEnemy, true);

			return bestEnemy;
		}
		
		public IEnvironmentEntity FindEnvironmentTarget()
		{
			if(_playerAutoAimController == null)
				return null;

			foreach (var damagableEnvEntity in _battleService.DamagableEnvEntities)
			{
				if (_battleService.PlayerEntity == null)
					continue;

				damagableEnvEntity.CurrentExecutionContext.TryResolveView(out IEnvironmentLiveView liveView);

				if (!liveView.IsAlive)
					continue;

				var dir = damagableEnvEntity.Transform.position - _battleService.PlayerEntity.Transform.position;
				var distance = dir.magnitude;

				if (!_playerAutoAimController.IsTargetInCameraView(damagableEnvEntity.Transform, distance))
					continue;
				
				return damagableEnvEntity;
			}

			return null;
		}
		
		public void AimAtTarget(ICharacterEntity target)
		{
			if(_playerAutoAimController == null)
				return;

			_playerAutoAimController.ApplyAutoAimHead(target);
			
			if(!_playerMovementController.IsMovingAnimPlaying)
				_playerMovementController.MoveToCurrentUncoverPoint();
		}
		
		public void AimAtTarget(Vector3 targetPosition)
		{
			if(_playerAutoAimController == null)
				return;

			_playerAutoAimController.ApplyAutoAim(targetPosition);
			
			if(_playerLiveView.IsImmortal)
				_playerStateMachineController.ForceShootState();
			
			if(!_playerMovementController.IsMovingAnimPlaying)
				_playerMovementController.MoveToCurrentUncoverPoint();
		}

		public int ShootEnemiesNowCount()
		{
			return _battleService.ShootingEnemiesAtCurrentGroup.Count;
		}

		public bool CalculateAndSetBestWeapon(ICharacterEntity enemyEntity)
		{
			return _bestWeaponModule.CalculateAndSetBestWeapon(enemyEntity);
		}
		
		public Transform CalculateBestShootTarget(ICharacterEntity enemyEntity)
		{
			if (_battleService.PlayerEntity == null)
				return null;
			
			_battleService.PlayerEntity.CurrentExecutionContext.TryResolveModel(out IItemsModel playerItemsModel);
			
			return _bestBodyPartModule.CalculateBestShootTarget(enemyEntity, playerItemsModel.CurrentWeaponFormat);
		}

		// ToDo: update according to new upgrades system (or remove?)
		public void UpgradeWeaponsToCurrentLevelPower()
		{
			/*if (_arsenalService.CurrentPrimaryWeapon != WeaponType.None)
			{
				while (_arsenalService.GetCurrentWeaponUpgradeStaticData(_arsenalService.CurrentPrimaryWeapon).UiUpgrades[WeaponUpgradeType.Power]
					   < _battleService.BattleContext.LevelStaticData.LevelData.TargetWeaponPower)
				{
					_arsenalService.UpgradeWeapon(_arsenalService.CurrentPrimaryWeapon, 1);
				}
			}

			if (_arsenalService.CurrentSecondaryWeapon != WeaponType.None)
			{
				while (_arsenalService.GetCurrentWeaponUpgradeStaticData(_arsenalService.CurrentSecondaryWeapon).UiUpgrades[WeaponUpgradeType.Power]
					   < _battleService.BattleContext.LevelStaticData.LevelData.TargetWeaponPower)
				{
					_arsenalService.UpgradeWeapon(_arsenalService.CurrentSecondaryWeapon, 1);
				}
			}*/
		}

		public void ResetHealthAmmoAndBattlePoints()
		{
			_raidSaveDataResetController.ResetRaidData();
		}

		public void EnterAimState()
		{
			if (_playerStateMachineModel.CurrentState is CoverState or null)
				_playerStateMachineController.ForceShootState();
		}
		
		public void ExitAimState()
		{
			if (_playerStateMachineModel.CurrentState is ShootStateBase or null)
				_playerStateMachineController.ForceCoverState();
		}
	}
}