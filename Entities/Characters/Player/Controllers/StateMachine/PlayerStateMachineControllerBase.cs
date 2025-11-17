using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Swat.Game.Entities.Bullets;
using Swat.Game.Entities.Characters.Common.Controllers;
using Swat.Game.Entities.Characters.Player.ExecutionContexts;
using Swat.Game.Entities.Characters.Player.Models;
using Swat.Game.Entities.Characters.Player.States;
using Swat.Game.Entities.Characters.Player.Views;
using Swat.Game.Entities.Core.Characters.Common.Controllers;
using Swat.Game.Entities.Core.Characters.Common.Views;
using Swat.Game.Entities.Core.Characters.Player.Controllers;
using Swat.Game.Entities.Core.Characters.Player.Models;
using Swat.Game.Entities.Core.Characters.Player.Views;
using Swat.Game.Entities.Core.Characters.States;
using Swat.Game.Entities.Core.Common.Models;
using Swat.Game.Entities.States;
using Swat.Game.GameControllers;
using Swat.Game.Services.BattleService;
using Swat.Game.Services.BattleService.Core;
using Swat.Game.UI.Windows.GameWindow.Core;
using Swat.Game.UI.Windows.GameWindow.Elements.Sniper.Core;
using CoverState = Swat.Game.Entities.States.CoverState;


namespace Swat.Game.Entities.Characters.Player.Controllers
{
	public abstract class PlayerStateMachineControllerBase : StateMachineController, ICharacterStateMachineBehaviourController
	{
		private const float InternalMoveStateDelay = 0.1f;
		private const float InternalMoveStateAfterReviveDelay = 2.0f;

		public override IState LastState
		{
			get => _playerStateMachineModel.LastState;
			protected set => _playerStateMachineModel.LastState = value;
		}

		public override IState CurrentState
		{
			get => _playerStateMachineModel.CurrentState;
			protected set => _playerStateMachineModel.CurrentState = value;
		}

		protected readonly IBattleService<BattleContext> BattleService;
		protected readonly IPlayerMovementView PlayerMovementView;
		protected readonly IPointerUiController PointerUiController;
		protected readonly ITwoFingersControlsUiController TwoFingersControlsUiController;
		protected readonly ISniperPanelUiController SniperPanelUiController;
		private readonly SceneEvents _sceneEvents;
		protected IPlayerMovementController MovementController;
		protected readonly IPlayerCoverTransitionModel PlayerCoverTransitionModel;

		protected abstract Type ShootStateType { get; }


		protected override bool AllowSameStateSwitch => false;
		private readonly ICharacterEntity _entity;
		private readonly IPlayerMovementModel _playerMovementModel;
		private readonly PlayerStateMachineView _playerStateMachineView;
		private readonly PlayerStateMachineModel _playerStateMachineModel;
		private readonly IPlayerInputController _playerInputController;
		private readonly ICharacterLiveView _characterLiveView;
		private readonly IItemsModel _itemsModel;
		private readonly IInteractabilityUiController _interactabilityUiController;
		private readonly ICrosshairUiController _crosshairUiController;
		protected IShootController<PlayerHitInfo> _playerShootController;

		private IPlayerCoverTransitionController _playerCoverTransitionController;
		private Tween _allEnemiesDeadTween;
		
		private bool _contextBegan;


		protected PlayerStateMachineControllerBase(ICollection<IState> states,
			IBattleService<BattleContext> battleService,
			ICharacterEntity entity,
			IPointerUiController pointerUiController,
			ITwoFingersControlsUiController twoFingersControlsUiController,
			ICrosshairUiController crosshairUiController,
			IPlayerMovementView playerMovementView,
			IPlayerMovementModel playerMovementModel,
			PlayerStateMachineView playerStateMachineView,
			PlayerStateMachineModel playerStateMachineModel,
			IPlayerInputController playerInputController,
			IPlayerCoverTransitionModel playerCoverTransitionModel,
			ICharacterLiveView characterLiveView,
			IItemsModel itemsModel, 
			IInteractabilityUiController interactabilityUiController,
			ISniperPanelUiController sniperPanelUiController) : base(states)
		{
			BattleService = battleService;
			_entity = entity;
			PointerUiController = pointerUiController;
			TwoFingersControlsUiController = twoFingersControlsUiController;
			_crosshairUiController = crosshairUiController;
			PlayerMovementView = playerMovementView;
			_playerMovementModel = playerMovementModel;
			_playerStateMachineView = playerStateMachineView;
			_playerStateMachineModel = playerStateMachineModel;
			_playerInputController = playerInputController;
			PlayerCoverTransitionModel = playerCoverTransitionModel;
			_characterLiveView = characterLiveView;
			_itemsModel = itemsModel;
			_interactabilityUiController = interactabilityUiController;
			SniperPanelUiController = sniperPanelUiController;
		}


		public override void OnContextBegin()
		{
			_entity.CurrentExecutionContext.TryResolveController(out MovementController);
			_entity.CurrentExecutionContext.TryResolveController(out _playerCoverTransitionController);
			_entity.CurrentExecutionContext.TryResolveController(out _playerShootController);
			
			_contextBegan = true;

			PointerUiController.OnPointerUpEvent += GameWindowControllerOnPointerUpEvent;
			PointerUiController.OnPointerDownEvent += GameWindowControllerOnPointerDownEvent;
			TwoFingersControlsUiController.OnCoverButtonDown += GameWindowControllerOnCoverButtonDown;
			TwoFingersControlsUiController.OnScopeButtonDown += GameWindowControllerOnScopeButtonDown;
			TwoFingersControlsUiController.OnJoystickDrag += GameWindowControllerOnJoystickDrag;
			_playerCoverTransitionController.OnCoverReached += OnCoverReachedHandler;
			_playerCoverTransitionController.OnCoverLeave += OnCoverLeaveHandler;
			_playerInputController.OnControlInputChanged += OnControlInputChanged;
			
			if (SniperPanelUiController != null)
			{
				SniperPanelUiController.OnSniperShootButtonUp += OnSniperShootButtonUp;
				SniperPanelUiController.OnSniperReload += OnSniperReload;
			}
		}


		public override void OnContextEnd()
		{
			PointerUiController.OnPointerUpEvent -= GameWindowControllerOnPointerUpEvent;
			PointerUiController.OnPointerDownEvent -= GameWindowControllerOnPointerDownEvent;
			TwoFingersControlsUiController.OnCoverButtonDown -= GameWindowControllerOnCoverButtonDown;
			TwoFingersControlsUiController.OnScopeButtonDown -= GameWindowControllerOnScopeButtonDown;
			TwoFingersControlsUiController.OnJoystickDrag -= GameWindowControllerOnJoystickDrag;
			_playerCoverTransitionController.OnCoverReached -= OnCoverReachedHandler;
			_playerCoverTransitionController.OnCoverLeave -= OnCoverLeaveHandler;
			_playerInputController.OnControlInputChanged -= OnControlInputChanged;
			
			if (SniperPanelUiController != null)
			{
				SniperPanelUiController.OnSniperShootButtonUp -= OnSniperShootButtonUp;
				SniperPanelUiController.OnSniperReload -= OnSniperReload;
			}
		}


		public override void Deinitialize()
		{
			base.Deinitialize();

			if(_contextBegan)
				OnContextEnd();
		}


		public void Activate<TInitialState>() where TInitialState : IState
		{
			SwitchState<TInitialState>();
		}

		public void ForceCoverState()
		{
			SwitchState<CoverState>();
		}

		public void ForceShootState()
		{
			SwitchState(ShootStateType);
		}


		protected virtual void GameWindowControllerOnPointerDownEvent()
		{
		}

		protected virtual void GameWindowControllerOnPointerUpEvent()
		{
		}

		protected virtual void GameWindowControllerOnCoverButtonDown()
		{
		}

		protected virtual void GameWindowControllerOnScopeButtonDown()
		{
			
		}

		protected virtual void GameWindowControllerOnJoystickDrag()
		{
		}

		protected virtual void OnSniperShootButtonUp()
		{
			_playerShootController?.TryShoot(true);
		}

		protected virtual void OnSniperReload()
		{
			_playerShootController?.TryForceReload(isManual: true);
		}


		protected virtual void OnCoverReachedHandler()
		{
			switch (CurrentState)
			{
				case MoveState:
					if (BattleService.MissionController.WasMissionComplete)
					{
						SwitchState<EndGameState>();
						return;
					}

					if (BattleService.AliveEnemiesAtCurrentGroup is { Count: 0 })
						return;

					PointerUiController.UnlockPointerHold();

					if (!PointerUiController.IsPointerHold)
						SwitchState<CoverState>();
					else
					{
						SwitchState<CoverState>();
						SwitchState(ShootStateType);
					}

					break;
			}
		}


		protected virtual void OnCoverLeaveHandler()
		{
			if (_allEnemiesDeadTween != null)
				return;

			switch (CurrentState)
			{
				case CoverState:
				case ShootStateBase:
					if (BattleService.MissionController.WasMissionComplete)
					{
						MovementController.SetRotationEnabled(false);
						_crosshairUiController.ShowCrossHair(false);
						_crosshairUiController.SetNoAmmoCrossEnabled(false);

						MovementController.MoveToUncoverPoint(PlayerCoverTransitionModel.CurrentCoverData);
						SwitchState<EndGameState>();
						return;
					}

					_entity.CurrentExecutionContext.ExecuteTask(LeaveCoverAsync);
					
					break;
			}
		}

		private async UniTask LeaveCoverAsync(CancellationToken ct)
		{
			float moveDelay = (CurrentState is ShootStateBase
							  ? _playerMovementModel.PreMoveStateDelay
							  : 0)
						  + (PlayerCoverTransitionModel.CurrentCoverData?.AdditionalLeaveCoverDelay ?? 0)
						  + InternalMoveStateDelay;

			if (_itemsModel.HasCurrentWeaponOpticalScope)
			{
				moveDelay += _playerMovementModel.AdditionalOpticalScopeMoveStateDelay;
				SwitchState<CoverState>();
				_interactabilityUiController.SetUiInteractable(false);
			}

			await UniTask.WaitForSeconds(moveDelay, cancellationToken: ct);

			if (!_characterLiveView.IsAlive)
			{
				float afterReviveDelay = _characterLiveView.IsAlive ? 0 : InternalMoveStateAfterReviveDelay;

				await UniTask.WaitUntil(() => _characterLiveView.IsAlive, cancellationToken: ct);
				await UniTask.WaitForSeconds(afterReviveDelay, cancellationToken: ct);
			}

			_interactabilityUiController.SetUiInteractable(true);
			
			SwitchState<MoveState>();
		}

		private void OnControlInputChanged(ControlInputType type)
		{
			if (type == ControlInputType.TwoFingers)
			{
				_entity.TrySwitchContext<PlayerTwoFingersControlsExecutionContext>();
			}
			else
			{
				_entity.TrySwitchContext<PlayerDefaultExecutionContext>();
			}

			MovementController.SetToCurrentCoverPoint();
		}
	}
}