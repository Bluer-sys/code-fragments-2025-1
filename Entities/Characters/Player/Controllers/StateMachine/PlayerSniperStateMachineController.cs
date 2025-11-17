using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Swat.Game.Entities.Characters.Player.Models;
using Swat.Game.Entities.Characters.Player.States;
using Swat.Game.Entities.Characters.Player.Views;
using Swat.Game.Entities.Core.Characters.Common.Views;
using Swat.Game.Entities.Core.Characters.Player.Models;
using Swat.Game.Entities.Core.Characters.Player.Views;
using Swat.Game.Entities.Core.Characters.States;
using Swat.Game.Entities.Core.Common.Models;
using Swat.Game.Entities.Core.Vehicle.Helicopter.Models;
using Swat.Game.Entities.States;
using Swat.Game.GameControllers.Events.Core;
using Swat.Game.Services.BattleService;
using Swat.Game.Services.BattleService.Data;
using Swat.Game.UI.Windows.GameWindow.Core;
using Swat.Game.UI.Windows.GameWindow.Elements.Sniper.Core;

namespace Swat.Game.Entities.Characters.Player.Controllers
{
	public class PlayerSniperStateMachineController : PlayerStateMachineControllerBase
	{
		protected override Type ShootStateType => typeof(AlternativeShootState);
		
		private readonly IGameplayUiController _gameplayUiController;
		private readonly IPlayerAnimationView _playerAnimationView;
		

		public PlayerSniperStateMachineController(
			ICollection<IState> states,
			IBattleService<BattleContext> battleService,
			ICharacterEntity entity,
			IPointerUiController pointerUiController,
			ITwoFingersControlsUiController twoFingersControlsUiController,
			ICrosshairUiController crosshairUiController,
			IPlayerMovementView playerMovementView,
			IPlayerAnimationView playerAnimationView,
			IPlayerMovementModel playerMovementModel,
			IPlayerInputController playerInputController,
			IGameplayUiController gameplayUiController,
			PlayerStateMachineView playerStateMachineView,
			PlayerStateMachineModel playerStateMachineModel,
			IPlayerCoverTransitionModel playerCoverTransitionModel,
			ICharacterLiveView characterLiveView,
			IItemsModel itemsModel,
			IInteractabilityUiController interactabilityUiController,
			ISniperPanelUiController sniperPanelUiController) 
			
			: base(
			states, 
			battleService, 
			entity, 
			pointerUiController, 
			twoFingersControlsUiController, 
			crosshairUiController, 
			playerMovementView, 
			playerMovementModel, 
			playerStateMachineView, 
			playerStateMachineModel, 
			playerInputController, 
			playerCoverTransitionModel, 
			characterLiveView, 
			itemsModel, 
			interactabilityUiController,
			sniperPanelUiController)
		{
			_gameplayUiController = gameplayUiController;
			_playerAnimationView = playerAnimationView;
		}


		public override void OnContextBegin()
		{
			base.OnContextBegin();

			_gameplayUiController.OnSettingsButtonClicked += OnOpenSettingsButtonClicked;
			SniperPanelUiController.OnScopeInStateChanged += OnScopeStateChanged;
			_playerAnimationView.OnReloadAnimationComplete += OnReloadComplete;
			_playerAnimationView.OnShootAnimationComplete += OnShootAnimationComplete;

			switch (CurrentState)
			{
				case MoveState:
					MovementController.SetAutoAlignRotationAsCoverPoint(true);
					break;

				case CoverState:
				case ShootState:
					MovementController.SetAutoAlignRotationAsCoverPoint(false);
					break;
			}
		}


		public override void OnContextEnd()
		{
			base.OnContextEnd();

			_gameplayUiController.OnSettingsButtonClicked -= OnOpenSettingsButtonClicked;
			SniperPanelUiController.OnScopeInStateChanged -= OnScopeStateChanged;
			_playerAnimationView.OnReloadAnimationComplete -= OnReloadComplete;
			_playerAnimationView.OnShootAnimationComplete -= OnShootAnimationComplete;
		}


		public override void Deinitialize()
		{
			base.Deinitialize();

			_gameplayUiController.OnSettingsButtonClicked -= OnOpenSettingsButtonClicked;
			SniperPanelUiController.OnScopeInStateChanged -= OnScopeStateChanged;
			_playerAnimationView.OnReloadAnimationComplete -= OnReloadComplete;
			_playerAnimationView.OnShootAnimationComplete -= OnShootAnimationComplete;
		}


		protected override void GameWindowControllerOnCoverButtonDown()
		{
			IHelicopterMovementModel helicopterMovementView = null;
			BattleService.PlayerVehicleEntity?.TryGetEntityModel(out helicopterMovementView);

			if (PlayerMovementView.IsMoving || helicopterMovementView is { IsMoving: true })
				return;

			switch (CurrentState)
			{
				case AlternativeShootState:
					SwitchState<CoverState>();
					break;

				case null:
				case CoverState:
					SwitchState<AlternativeShootState>();
					break;
			}
		}
		

		protected override void OnCoverLeaveHandler()
		{
			switch (CurrentState)
			{
				case CoverState:
				case ShootStateBase:
					MovementController.SetAutoAlignRotationAsCoverPoint(true);
					break;
			}
		}


		protected override void OnCoverReachedHandler()
		{
			switch (CurrentState)
			{
				case MoveState:
					MovementController.SetAutoAlignRotationAsCoverPoint(false);
					break;
			}

			base.OnCoverReachedHandler();
		}
		
		
		private void OnShootAnimationComplete()
		{
			_playerShootController.TryForceReload();
		}
		
		
		protected override void OnSniperReload()
		{
			var isReloadPossible = _playerShootController.TryForceReload(isManual: true);
			
			if (isReloadPossible)
				SniperPanelUiController.PlayReloadAnimation(_playerShootController.GetCurrentReloadTime());
		}


		private void OnOpenSettingsButtonClicked()
		{
			if (CurrentState is AlternativeShootState)
				GameWindowControllerOnCoverButtonDown();
		}
		
		
		private void OnReloadComplete()
		{
			if (CurrentState is AlternativeShootState)
				ScopeInAfterDelay().Forget();

			async UniTask ScopeInAfterDelay()
			{
				await UniTask.WaitForFixedUpdate();
				_playerShootController.ScopeIn();
			}
		}
		
		
		private void OnScopeStateChanged(bool isScopeIn)
		{
			if (isScopeIn)
			{
				MovementController.SetAutoAlignRotationAsCoverPoint(false);
				SwitchState<AlternativeShootState>();
				_playerShootController.ScopeIn();
			}
			else
			{
				SwitchState<CoverState>();
			}
		}
		

	}
}