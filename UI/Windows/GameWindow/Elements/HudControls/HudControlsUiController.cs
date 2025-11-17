using System;
using Swat.Game.Entities;
using Swat.Game.Services.BattleService;
using Swat.Game.Services.BattleService.Core;
using Swat.Game.Services.ScreenOrientationService.Core;
using Swat.Game.Services.WindowsService;
using Swat.Game.UI.Windows.ControlsSettings.Core;
using Swat.Game.UI.Windows.ControlsSettings.Data;
using Swat.Game.UI.Windows.GameWindow.Core;
using UnityEngine;
using Zenject;

namespace Swat.Game.UI.Windows.GameWindow.Elements.HudControls
{
	[Serializable]
	public class HudControlsUiController : BaseUiController<HudControlsUi>, IHudControlsUiController
	{
		private IPlayerInputController _playerInputController;
		private IScreenOrientationService _windowsService;


		[Inject]
		public void Construct(IBattleService<BattleContext> battleService,
			IPlayerInputController playerInputController,
			IScreenOrientationService windowsService,
			IControlsSettingsController controlsSettingsController)
		{
			_playerInputController = playerInputController;
			_windowsService = windowsService;

			battleService.OnPlayerAuthorized += BattleServiceOnPlayerAuthorized;
			controlsSettingsController.OnHudElementsChanged += OnHudElementsChanged;
			playerInputController.OnControlInputChanged += OnControlInputChanged;
		}

		
		public override void OnChangeOrientation(UiOrientation uiOrientation)
		{
			base.OnChangeOrientation(uiOrientation);
			Refresh();
		}


		private void BattleServiceOnPlayerAuthorized(ICharacterEntity playerEntity)
		{
			Refresh();
		}


		private void OnHudElementsChanged()
		{
			Refresh();
		}


		private void OnControlInputChanged(ControlInputType inputType)
		{
			Refresh();
		}


		private void Refresh()
		{
			RefreshControlsScheme();

			view.ActivateControls(_playerInputController.ControlInputType == ControlInputType.TwoFingers);
			view.SetCoverButtonActive(_playerInputController.CoverInputType == CoverInputType.ControlCover);
		}


		private void RefreshControlsScheme()
		{
			foreach ((ControlElement type, RectTransform transform) in view.Controls)
				transform.anchoredPosition = _playerInputController.GetHudPositions(_windowsService.UiOrientation)[type];
		}
	}
}