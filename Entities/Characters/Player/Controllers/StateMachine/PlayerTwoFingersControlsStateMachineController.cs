using System;
using System.Collections.Generic;
using DG.Tweening;
using Swat.Game.Core;
using Swat.Game.Entities.Characters.Player.Models;
using Swat.Game.Entities.Characters.Player.States;
using Swat.Game.Entities.Characters.Player.Views;
using Swat.Game.Entities.Core.Characters.Common.Views;
using Swat.Game.Entities.Core.Characters.Player.Controllers;
using Swat.Game.Entities.Core.Characters.Player.Models;
using Swat.Game.Entities.Core.Characters.Player.Views;
using Swat.Game.Entities.Core.Characters.States;
using Swat.Game.Entities.Core.Common.Models;
using Swat.Game.Entities.Core.Vehicle.Helicopter.Models;
using Swat.Game.Entities.States;
using Swat.Game.Services.BattleService;
using Swat.Game.Services.BattleService.Core;
using Swat.Game.UI.Windows.GameWindow.Core;
using Swat.Game.UI.Windows.GameWindow.Elements.Sniper.Core;
using Swat.Utils;
using UnityEngine;
using CoverState = Swat.Game.Entities.States.CoverState;


namespace Swat.Game.Entities.Characters.Player.Controllers
{
	public class PlayerTwoFingersControlsStateMachineController : PlayerStateMachineControllerBase, IUpdatable
	{
		protected override Type ShootStateType => typeof(AlternativeShootState);
		private readonly ICharacterEntity _entity;
		private readonly IPlayerInputController playerInputController;
		private readonly IGameplayUiController gameplayUiController;
		private readonly IPlayerShootModel _playerShootModel;
		private ICharacterLiveView characterLiveView;
		
		private Tween allEnemiesDead;
		private IPlayerMovementController _playerMovementController;
		private Tween delayedCoverTween;

		public PlayerTwoFingersControlsStateMachineController(
			ICollection<IState> states,
			IBattleService<BattleContext> battleService,
			ICharacterEntity entity,
			IPointerUiController pointerUiController,
			ITwoFingersControlsUiController twoFingersControlsUiController,
			ICrosshairUiController crosshairUiController,
			IPlayerMovementView playerMovementView,
			IPlayerMovementModel playerMovementModel,
			IPlayerInputController playerInputController,
			IGameplayUiController gameplayUiController,
			PlayerStateMachineView playerStateMachineView,
			PlayerStateMachineModel playerStateMachineModel,
			IPlayerCoverTransitionModel playerCoverTransitionModel,
			ICharacterLiveView characterLiveView,
			IItemsModel itemsModel,
			IInteractabilityUiController interactabilityUiController,
			ISniperPanelUiController sniperPanelUiController,
			IPlayerShootModel playerShootModel) 
			
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
			_entity = entity;
			this.playerInputController = playerInputController;
			this.gameplayUiController = gameplayUiController;
			_playerShootModel = playerShootModel;
		}


		public override void OnContextBegin()
		{
			base.OnContextBegin();

			gameplayUiController.OnSettingsButtonClicked += OnOpenSettingsButtonClicked;
			TwoFingersControlsUiController.OnShootButtonDown += OnShoot;
			
			_entity.CurrentExecutionContext.TryResolveView(out characterLiveView);
			_entity.CurrentExecutionContext.TryResolveController(out _playerMovementController);
			
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

			TwoFingersControlsUiController.OnShootButtonDown -= OnShoot;
			gameplayUiController.OnSettingsButtonClicked -= OnOpenSettingsButtonClicked;
		}


		public override void Deinitialize()
		{
			base.Deinitialize();

			gameplayUiController.OnSettingsButtonClicked -= OnOpenSettingsButtonClicked;
		}


		public void OnUpdate()
		{
			HandleAutoCover();
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
					if (_playerShootModel.IsSingleShotMode && PlayerCoverTransitionModel.HasCover)
						RunDelayedCover();
					else
						SwitchState<CoverState>();
					break;

				case null:
				case CoverState:
					SwitchState<AlternativeShootState>();
					break;
			}
		}

		protected override void GameWindowControllerOnScopeButtonDown()
		{
			// In cover: toggle scope without switching state; block while moving between positions
			switch (CurrentState)
			{
				case CoverState:
					if (_playerMovementController != null && _playerMovementController.IsMovingOrRotating())
						return;
					_playerShootController.SwitchScope();
					return;
				default:
					_playerShootController.SwitchScope();
					break;
			}
	}

		protected override void OnCoverLeaveHandler()
		{
			base.OnCoverLeaveHandler();

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


		private void OnOpenSettingsButtonClicked()
		{
			if (CurrentState is AlternativeShootState)
				GameWindowControllerOnCoverButtonDown();
		}


		private void HandleAutoCover()
		{
			IHelicopterMovementModel helicopterMovementModel = null;
			BattleService.PlayerVehicleEntity?.TryGetEntityModel(out helicopterMovementModel);

			if (playerInputController.CoverInputType != CoverInputType.AutoCover
				|| helicopterMovementModel is { IsMoving: true }
				|| _playerMovementController.IsMovingOrRotating())
				return;

			int touchCount = TwoFingersControlsUiController.IsShootButtonPressed.ToInt()
							 + TwoFingersControlsUiController.IsJoystickButtonPressed.ToInt()
							 + _playerShootModel.IsInScope.ToInt()
							 + PointerUiController.IsPointerHold.ToInt()
							 + (!PlayerCoverTransitionModel.HasCover).ToInt()
							 #if UNITY_EDITOR
							 + Input.GetKey(KeyCode.LeftShift).ToInt()
							 #endif
							 ;

			touchCount = characterLiveView.IsAlive ? touchCount : 0;
			
			switch (CurrentState)
			{
				case AlternativeShootState when touchCount == 0:
					if (_playerShootModel.IsSingleShotMode && PlayerCoverTransitionModel.HasCover)
						RunDelayedCover();
					else
						SwitchState<CoverState>();

					break;

				case CoverState or null when touchCount > 0:
					MovementController.SetAutoAlignRotationAsCoverPoint(false);
					SwitchState<AlternativeShootState>();
					break;
			}
		}


		public override void SwitchState(IState state)
		{
			base.SwitchState(state);
			delayedCoverTween?.Kill();
		}
		
		
		private void OnShoot()
		{
			if (delayedCoverTween == null || !delayedCoverTween.IsPlaying())
				return;
				
			RunDelayedCover(true);
		}

		private void RunDelayedCover(bool forced = false)
		{
			if (!forced && delayedCoverTween != null && delayedCoverTween.IsPlaying())
				return;
			
			delayedCoverTween?.Kill();
			delayedCoverTween = DOVirtual.DelayedCall(_playerShootModel.SingleShotModeUncoverDelay, GoToCover);
		}


		private void GoToCover() =>
			SwitchState<CoverState>();
	}
}
