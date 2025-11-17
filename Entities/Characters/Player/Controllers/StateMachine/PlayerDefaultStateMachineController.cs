using System;
using System.Collections.Generic;
using DG.Tweening;
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
using Swat.Game.Services.BattleService;
using Swat.Game.UI.Windows.GameWindow.Core;
using Swat.Game.UI.Windows.GameWindow.Elements.Sniper.Core;
using CoverState = Swat.Game.Entities.States.CoverState;


namespace Swat.Game.Entities.Characters.Player.Controllers
{
	public class PlayerDefaultStateMachineController : PlayerStateMachineControllerBase
	{
		protected override Type ShootStateType => typeof(ShootState);
		private Tween allEnemiesDead;


		public PlayerDefaultStateMachineController(
			ICollection<IState> states,
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
		}

		public override void OnContextBegin()
		{
			base.OnContextBegin();

			MovementController.SetAutoAlignRotationAsCoverPoint(true);
		}


		protected override void GameWindowControllerOnPointerDownEvent()
		{
			IHelicopterMovementModel helicopterMovementModel = null;
			BattleService.PlayerVehicleEntity?.TryGetEntityModel(out helicopterMovementModel);

			if (helicopterMovementModel is { IsMoving: true })
				return;

			switch (CurrentState)
			{
				case null:
				case CoverState:
					SwitchState<ShootState>();
					break;
			}
		}


		protected override void GameWindowControllerOnPointerUpEvent()
		{
			switch (CurrentState)
			{
				case ShootState:
					if (allEnemiesDead != null && allEnemiesDead.IsPlaying())
					{
						allEnemiesDead?.Kill();
						SwitchState<MoveState>();
						return;
					}

					SwitchState<CoverState>();
					return;
			}
		}
	}
}
