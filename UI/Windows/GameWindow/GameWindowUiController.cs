using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Swat.Game.Entities.Core.Common.Models;
using Swat.Game.Services.AudioService;
using Swat.Game.Services.BattleService;
using Swat.Game.Services.LevelService.Data;
using Swat.Game.Services.ScreenOrientationService.Core;
using Swat.Game.UI.Panels.Core;
using Swat.Game.UI.Panels.InGameBonusesPanel.Core;
using Swat.Game.UI.Windows.EndGameWindow.Core;
using Swat.Game.UI.Windows.EndGameWindow.Data;
using Swat.Game.UI.Windows.GameWindow.Core;
using Swat.Game.UI.Windows.GameWindow.Elements.EnemyDetector.Core;
using Swat.Game.UI.Windows.GameWindow.Elements.Sniper.Core;
using Zenject;

namespace Swat.Game.Services.WindowsService.Windows
{
	public class GameWindowUiController : AnimatableBaseUiController<GameWindowUiModel, GameWindow>,
		IInteractabilityUiController,
		IGameplayUiController
	{
		public event Action OnSettingsButtonClicked;
		public event Action<UiOrientation> OnChangeGameplayOrientation;
		public event Action<bool> OnUiInteractableChanged;


		public UiOrientation CurrentGameplayOrientation => _screenOrientationService.UiOrientation;


		private IBattleService<BattleContext> _battleService;
		private IWindowsService _windowsService;
		private IScreenOrientationService _screenOrientationService;
		private IAudioService<SoundId> _audioService;
		private IGameWindowUniversalController _gameWindowUniversalController;
		private IInGameBonusesPanelUiController _inGameBonusesPanelUiController;
		private IPointerUiController _pointerUiController;
		private IEnemyUiController _enemyUiController;
		private ILiveUiController _liveUiController;
		private IFarmMissionTimerUiController _farmMissionTimerUiController;
		private IFarmMissionScoreUIController _farmMissionScoreUIController;
		private IGameModesUiOnOffController _gameModesUiOnOffController;


		private Queue<SoundId> startLevelTalkingQueue;
		private Queue<SoundId> startLevelTalkingQueueChase;
		private Queue<SoundId> startLevelTalkingQueueSniper;
		private Queue<SoundId> victoryLevelTalkingQueue;
		private Tween startTalkingDelayTween;
		private IEndgameWindowUiController _endgameWindowUiController;
		private IEnemyDetectorUiController _enemyDetectorUiController;


		[Inject]
		public void Construct(IBattleService<BattleContext> battleService,
			IWindowsService windowsService,
			IScreenOrientationService screenOrientationService,
			IAudioService<SoundId> audioService,
			IGameWindowUniversalController gameWindowUniversalController,
			IInGameBonusesPanelUiController inGameBonusesPanelUiController,
			IPointerUiController pointerUiController,
			IEnemyUiController enemyUiController,
			ILiveUiController liveUiController,
			IFarmMissionTimerUiController farmMissionTimerUiController,
			IFarmMissionScoreUIController farmMissionScoreUIController,
			IGameModesUiOnOffController gameModesUiOnOffController,
			IEndgameWindowUiController endgameWindowUiController,
			IEnemyDetectorUiController enemyDetectorUiController)
		{
			_enemyDetectorUiController = enemyDetectorUiController;
			_endgameWindowUiController = endgameWindowUiController;
			this._battleService = battleService;
			this._windowsService = windowsService;
			this._screenOrientationService = screenOrientationService;
			this._audioService = audioService;
			
			_gameWindowUniversalController = gameWindowUniversalController;
			_inGameBonusesPanelUiController = inGameBonusesPanelUiController;
			_pointerUiController = pointerUiController;
			_enemyUiController = enemyUiController;
			_liveUiController = liveUiController;
			_farmMissionTimerUiController = farmMissionTimerUiController;
			_farmMissionScoreUIController = farmMissionScoreUIController;
			_gameModesUiOnOffController = gameModesUiOnOffController;

			startLevelTalkingQueue = new Queue<SoundId>(model.StartLevelTalking);
			startLevelTalkingQueueChase = new Queue<SoundId>(model.StartLevelTalkingChase);
			startLevelTalkingQueueSniper = new Queue<SoundId>(model.StartLevelTalkingSniper);
			victoryLevelTalkingQueue = new Queue<SoundId>(model.VictoryLevelTalking);
		}


		public override async UniTask OnShowBeginAsync()
		{
			view.RefreshSafeArea();
			SetVisible(true);
			
			_gameModesUiOnOffController.SetCurrentGameModeUi();
			_inGameBonusesPanelUiController.OnShowBegin();
			_gameWindowUniversalController.OnShowBegin();
			_enemyUiController.OnShowBegin();
			_liveUiController.OnShowBegin();
			_farmMissionTimerUiController.OnShowBegin();
			_farmMissionScoreUIController.OnShowBegin();
			_enemyDetectorUiController.OnShowBegin();
			
			await base.OnShowBeginAsync();
			await view.PlayShowAnimation();
		}


		public override async UniTask OnShowEndAsync()
		{
			await base.OnShowEndAsync();
			_windowsService.TryShowWindow(WindowType.TutorialWindow);
		}

		public override async UniTask OnHideBeginAsync()
		{
			await view.PlayHideAnimation();
			await base.OnHideBeginAsync();
		}


		public override void RefreshViewAfterChangeOrientation()
		{
			base.RefreshViewAfterChangeOrientation();
			view.OnPauseClicked += OnPauseClicked;
		}


		public override void RefreshViewBeforeChangeOrientation()
		{
			base.RefreshViewBeforeChangeOrientation();
			view.OnPauseClicked -= OnPauseClicked;
		}

		
		private void OnPauseClicked()
		{
			_windowsService.TryShowWindow(WindowType.Pause);
		}


		public void OnMissionComplete()
		{
			_battleService.PlayerEntity.TryGetEntityModel(out IItemsModel itemsModel);

			_windowsService.TryHideWindow(WindowType.Settings);
			_windowsService.TryHideWindow(WindowType.ControlsSettings);

			if (itemsModel.HasRewardWeapon)
			{
				PlayNewGunTalking();
			}
			else
			{
				PlayVictoryTalking();
				_windowsService.TryHideWindow(WindowType.TutorialWindow);
			}

			if (_battleService.MissionController.DontShowVictoryWindow)
			{
				_endgameWindowUiController.ForceClickContinue();
			}
			else
			{
				_windowsService.TryPlayWindowTransition(WindowType.Game, WindowType.EndGame, TransitionType.Animation,
					null, new object[] { EndGameShowOption.Victory });
			}
		}

		public void OnMissionFailed()
		{
			_windowsService.TryHideWindow(WindowType.TutorialWindow);
			_windowsService.TryPlayWindowTransition(WindowType.Game, WindowType.EndGame, TransitionType.Animation, null, new object[] { EndGameShowOption.Defeat });
		}

		public void SetVisible(bool isVisible)
		{
			view.gameObject.SetActive(isVisible);
		}


		public void SetUiInteractable(bool isInteractable)
		{
			view.VisibilityCanvasGroup.interactable = isInteractable;
			view.GraphicRaycaster.enabled = isInteractable;
			OnUiInteractableChanged?.Invoke(isInteractable);
		}
		
		
		public void SetUiVisible(bool visible)
		{
			view.VisibilityCanvasGroup.alpha = visible ? 1 : 0;
		}


		public override void OnChangeOrientation(UiOrientation uiOrientation)
		{
			base.OnChangeOrientation(uiOrientation);
			OnChangeGameplayOrientation?.Invoke(uiOrientation);
			view.RefreshSafeArea();
		}


		private void PlayStartTalking()
		{
			switch (_battleService.BattleContext.LevelType)
			{
				case LevelType.Sniper:
					Play(ref startLevelTalkingQueueSniper);
					break;

				case LevelType.ChaseBackward:
				case LevelType.ChaseForward:
					Play(ref startLevelTalkingQueueChase);
					break;

				default:
					Play(ref startLevelTalkingQueue);
					break;
			}


			void Play(ref Queue<SoundId> queue)
			{
				_audioService.PlayUiSound(queue.Dequeue());

				if (queue.Count == 0)
					queue = new Queue<SoundId>(model.StartLevelTalking);
			}
		}


		private void PlayVictoryTalking()
		{
			_audioService.PlayUiSound(victoryLevelTalkingQueue.Dequeue());

			if (victoryLevelTalkingQueue.Count == 0)
				victoryLevelTalkingQueue = new Queue<SoundId>(model.VictoryLevelTalking);
		}


		private void PlayNewGunTalking()
		{
			_audioService.PlayUiSound(SoundId.VoicePlayerVictoryNewGun);
		}
	}
}
