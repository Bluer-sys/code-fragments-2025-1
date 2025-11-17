using System;
using DG.Tweening;
using Dreamteck.Splines;
using JetBrains.Annotations;
using Swat.Game.Core;
using Swat.Game.Entities.Characters.Player.Models;
using Swat.Game.Entities.Characters.Player.Views;
using Swat.Game.Entities.Core.Characters.Player.Controllers;
using Swat.Game.GameControllers;
using Swat.Game.GameControllers.Data;
using Swat.Game.GameControllers.Scenario;
using Swat.Game.Services.BattleService;
using Swat.Game.Services.WindowsService;
using Swat.Game.UI.Windows.CutsceneWindow.Core;
using Swat.Game.UI.Windows.PreGameWindow.Core;
using UnityEngine;
using UnityEngine.Playables;

namespace Swat.Game.Entities.Characters.Player.Controllers
{
	public class PlayerCutsceneController : BaseController, IPlayerCutsceneController, ILateUpdatable
	{
		public event Action OnCutsceneStopped;
		public event Action OnCutscenePlay;

		private readonly ICharacterEntity _entity;
		private readonly PlayerCutsceneModel _model;
		private readonly PlayerCutsceneView _view;
		private readonly SceneEvents _sceneEvents;
		private readonly IWindowsService _windowsService;
		private readonly IBattleService<BattleContext> _battleService;
		private readonly ICutsceneWindowUiController _cutsceneWindowUiController;
		private readonly IPreGameUiController _preGameUiController;

		private IPlayerCutsceneCameraController _cutsceneCameraController;
		private IPlayerMovementController _playerMovementController;
		private IPlayerAnimationController _playerAnimationController;
		private Cutscene _cutscene;
		private bool _isManualStop;

		public PlayerCutsceneController(ICharacterEntity entity,
			PlayerCutsceneModel model,
			PlayerCutsceneView view,
			SceneEvents sceneEvents,
			IWindowsService windowsService,
			IBattleService<BattleContext> battleService,
			ICutsceneWindowUiController cutsceneWindowUiController,
			IPreGameUiController preGameUiController)
		{
			_entity = entity;
			_model = model;
			_view = view;
			_sceneEvents = sceneEvents;
			_windowsService = windowsService;
			_battleService = battleService;
			_cutsceneWindowUiController = cutsceneWindowUiController;
			_preGameUiController = preGameUiController;
		}


		public override void Initialize()
		{
			_entity.TryGetEntityController(out _cutsceneCameraController);
			_entity.TryGetEntityController(out _playerMovementController);
			_entity.TryGetEntityController(out _playerAnimationController);
			
			_sceneEvents.OnCutsceneLaunch += PlayCutscene;
			_sceneEvents.OnCutsceneDialogChanged += ChangeDialog;

			_cutsceneWindowUiController.OnSkipCutsceneClicked += SkipCutscene;
		}


		public override void Deinitialize()
		{
			UnsubscribeLastCutscene();

			_sceneEvents.OnCutsceneLaunch -= PlayCutscene;
			_sceneEvents.OnCutsceneDialogChanged -= ChangeDialog;
			
			_cutsceneWindowUiController.OnSkipCutsceneClicked -= SkipCutscene;
		}


		public void PlayCutscene(Cutscene cutscene)
		{
			_isManualStop = false;
			UnsubscribeLastCutscene();

			_cutscene = cutscene;
			var director = cutscene.Data.Director;

			if (_cutscene.Data.DisableGameplayHands)
				_playerAnimationController.SetHandsVisible(false);

			_playerMovementController.SetStepSoundActive(false);
			
			_cutsceneCameraController.ActiveCutsceneCamera(cutscene.Data.VirtualCamera);
			DOVirtual.DelayedCall(0.1f, ShowCutsceneWindow);
			
			director.playOnAwake = false;
			director.RebuildGraph();
			
			director.playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime); // GameTime need for correct work of Signals
			director.timeUpdateMode = DirectorUpdateMode.GameTime;
			
			director.stopped += OnCutsceneStoppedHandler;
			director.Play();
			
			OnCutscenePlay?.Invoke();
		}

		public void OnLateUpdate()
		{
			if (_cutscene == null || !_cutscene.Data.Director.playableGraph.IsValid())
				return;

			_cutsceneCameraController.ManualUpdate();
		}

		private void ShowCutsceneWindow()
		{
			if (_cutscene != null && !_windowsService.IsWindowActive(WindowType.Cutscene))
				_windowsService.TryPlayWindowTransition(WindowType.Game, WindowType.Cutscene, TransitionType.Animation);
		}

		private void OnCutsceneStoppedHandler(PlayableDirector director)
		{
			if (_isManualStop)
				return;
			
			if (_cutscene.Data.AfterCutsceneEndLevel && !_isManualStop)
			{
				_sceneEvents.OnLevelEndReached();
				return;
			}

			if (!_cutscene.Data.AfterCutsceneEndLevel)
				_playerMovementController.SetStepSoundActive(true);
			
			if (_cutscene.Data.DisableGameplayHands)
				_playerAnimationController.SetHandsVisible(true);
			
			AttachToSpline();
			_cutsceneCameraController.DeactivateCutsceneCamera(onComplete: () =>
			{
				ShowGameWindow();

				_cutscene.Data.VirtualCamera.gameObject.SetActive(false);
				OnCutsceneStopped?.Invoke();

				_cutscene = null;
			});
		}

		private void SkipCutscene()
		{
			if (_cutscene == null)
			{
				ShowGameWindow();
				return;
			}
			
			if (_cutscene.Data.AfterCutsceneEndLevel)
			{
				_isManualStop = true;
				_sceneEvents.OnLevelEndReached();
				_cutscene.Data.Director.Pause();
				return;
			}
			
			var director = _cutscene.Data.Director;

			director.timeUpdateMode = DirectorUpdateMode.Manual;
			director.playableGraph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

			director.playableGraph.Evaluate(float.MaxValue);
			
			director.Stop();
		}

		private void ChangeDialog(string dialogId)
		{
			if (_cutscene == null)
				return;
			
			var data = _cutscene.GetDialogData(dialogId);
			_cutsceneWindowUiController.SetCutsceneDialog(data);
		}

		private void AttachToSpline()
		{
			if(_cutscene == null || _cutscene.Data.AfterCutsceneSpline == null)
				return;
			
			var spline = _cutscene.Data.AfterCutsceneSpline;
			var sample = new SplineSample();
			spline.Evaluate(0f, ref sample);
			var position = spline.GetPoint(0).position;

			_entity.Transform.position = position;
			_playerMovementController.SetSplineFollowerPercent(0);
		}

		private void UnsubscribeLastCutscene()
		{
			if (_cutscene != null)
				_cutscene.Data.Director.stopped -= OnCutsceneStoppedHandler;
		}

		private void ShowGameWindow()
		{
			_windowsService.TryPlayWindowTransition(WindowType.Cutscene, _preGameUiController.ShouldShowPreGameUi? WindowType.PreGameWindow : WindowType.Game, TransitionType.Animation);
		}
	}
}
