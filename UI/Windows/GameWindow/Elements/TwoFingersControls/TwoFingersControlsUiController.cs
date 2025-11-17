using System;
using Swat.Game.Entities;
using Swat.Game.Entities.Bullets;
using Swat.Game.Entities.Characters.Player.Controllers;
using Swat.Game.Entities.Characters.Player.Views;
using Swat.Game.Entities.Core.Characters.Common.Controllers;
using Swat.Game.Entities.Core.Characters.Player.Controllers;
using Swat.Game.Entities.Core.Characters.Player.Models;
using Swat.Game.Entities.Core.Characters.Player.Views;
using Swat.Game.Services.BattleService;
using Swat.Game.Services.WindowsService;
using Swat.Game.UI.Windows.GameWindow.Core;
using UnityEngine;
using Zenject;

namespace Swat.Game.UI.Windows.GameWindow.Elements.TwoFingersControls
{
	[Serializable]
	public class TwoFingersControlsUiController : BaseUiController<TwoFingersControlsUi>, ITwoFingersControlsUiController
	{
		public event Action OnShootButtonDown;
		public event Action OnShootButtonUp;
		public event Action OnCoverButtonDown;
		public event Action OnScopeButtonDown;
		public event Action OnJoystickDrag;

		public Vector2 JoystickDirection => view.Joystick.InputVector;
		public Vector2 JoystickDelta => view.Joystick.InputVectorPixels;

		public bool IsJoystickButtonPressed => view.Joystick.IsDragged;
		public bool IsShootButtonPressed { get; private set; }
	

		private IBattleService<BattleContext> battleService;

		private IPlayerMovementView _playerMovementView;
		private IPlayerMovementModel _playerMovementModel;
		private IPlayerMovementController _playerMovementController;
		private IShootController<PlayerHitInfo> _playerShootController;


		public override void RefreshViewBeforeChangeOrientation()
		{
			base.RefreshViewBeforeChangeOrientation();

			view.CoverButton.Button.onClick.RemoveListener(OnCoverClicked);
			view.ScopeButton.Button.onClick.RemoveListener(OnScopeClicked);
			view.ShootButton.OnPointerDownEvent -= OnShootDown;
			view.ShootButton.OnPointerUpEvent -= OnShootUp;
			view.Joystick.OnJoystickDrag -= OnJoystickDragHandler;
			battleService.OnPlayerAuthorized -= BattleServiceOnPlayerAuthorized;
			battleService.OnPlayerDeAuthorized -= BattleServiceOnPlayerDeAuthorized;
			battleService.OnBattleBegin -= Reset;
			battleService.OnBattleEnd -= Reset;
			battleService.OnBattleRestart -= Reset;
		}


		public override void RefreshViewAfterChangeOrientation()
		{
			base.RefreshViewAfterChangeOrientation();

			view.CoverButton.Button.onClick.AddListener(OnCoverClicked);
			view.ScopeButton.Button.onClick.AddListener(OnScopeClicked);
			view.ShootButton.OnPointerDownEvent += OnShootDown;
			view.ShootButton.OnPointerUpEvent += OnShootUp;
			view.Joystick.OnJoystickDrag += OnJoystickDragHandler;
			battleService.OnPlayerAuthorized += BattleServiceOnPlayerAuthorized;
			battleService.OnPlayerDeAuthorized += BattleServiceOnPlayerDeAuthorized;
			battleService.OnBattleBegin += Reset;
			battleService.OnBattleEnd += Reset;
			battleService.OnBattleRestart += Reset;
			
			RefreshCoverButton();
		}


		[Inject]
		private void Construct(IBattleService<BattleContext> battleService)
		{
			this.battleService = battleService;
		}


		private void BattleServiceOnPlayerAuthorized(ICharacterEntity playerEntity)
		{
			playerEntity.TryGetEntityView(out _playerMovementView);
			playerEntity.TryGetEntityModel(out _playerMovementModel);
			playerEntity.TryGetEntityController(out _playerMovementController);
			playerEntity.TryGetEntityController(out _playerShootController);

			_playerShootController.OnScopeStatusChanged += RefreshScopeButton;
			
			Reset();
		}


		private void BattleServiceOnPlayerDeAuthorized(ICharacterEntity playerEntity)
		{
			playerEntity.TryGetEntityController(out _playerMovementController);
			playerEntity.TryGetEntityController(out _playerShootController);
			
			_playerShootController.OnScopeStatusChanged -= RefreshScopeButton;

			_playerMovementView = null;
			_playerMovementController = null;
			_playerShootController = null;
			_playerMovementModel = null;

			Reset();
		}


		private void OnShootDown()
		{
			IsShootButtonPressed = true;
			OnShootButtonDown?.Invoke();
		}


		private void OnShootUp()
		{
			IsShootButtonPressed = false;
			OnShootButtonUp?.Invoke();
		}


		private void OnJoystickDragHandler()
		{
			OnJoystickDrag?.Invoke();
		}


		private void OnCoverClicked()
		{
			OnCoverButtonDown?.Invoke();

			RefreshCoverButton();
		}

		private void OnScopeClicked()
		{
			OnScopeButtonDown?.Invoke();
		}

		public void RefreshScopeButton(bool isInScope)
		{
			var state = isInScope ? ButtonState.Pressed : ButtonState.Normal;

			foreach (var pair in view.ScopeButtonStates)
				pair.Value.SetActive(pair.Key == state);
		}

		public void Reset()
		{
			IsShootButtonPressed = false;
		}
	}
}
