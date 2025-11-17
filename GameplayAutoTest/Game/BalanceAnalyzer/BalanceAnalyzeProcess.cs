using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using Swat.Game.Data.Reward;
using Swat.Game.Data.Weapon;
using Swat.Game.Entities.Core.Characters.Common.Views;
using Swat.Game.Entities.Core.Weapons.Models;
using Swat.Game.GameControllers.Events.Core;
using Swat.Game.GameplayRobot.Core.Interfaces;
using Swat.Game.GameplayRobot.Core.Process;
using Swat.Game.GameplayRobot.Game.BalanceAnalyzer.Recorder;
using Swat.Game.Services.ArsenalService.Core;
using Swat.Game.Services.BattleService;
using Swat.Game.Services.BattleStatisticsService.Core;
using Swat.Game.Services.CurrencyService.Core;
using Swat.Game.Services.LevelService;
using Swat.Game.Services.LevelService.Core;
using Swat.Game.Services.WindowsService;
using Swat.Game.Utils;
using UnityEngine;

namespace Swat.Game.GameplayRobot.Game.BalanceAnalyzer
{
	public class BalanceAnalyzeProcess : RobotProcess<BalanceAnalyzeConfiguration>
	{
		private readonly FpsCounter _fpsCounter;
		private readonly IBattleStatisticsService _battleStatisticsService;
		private readonly ILevelServiceOld<StageStaticData, LevelStaticData> _levelServiceOld;
		private readonly IWindowsService _windowsService;
		private readonly IBattleService<BattleContext> _battleService;
		private readonly IArsenalService<WeaponStaticData> _arsenalService;
		private readonly IPlayerEvents _playerEvents;

		private readonly List<AnalyzeRecorder> _analyzePresets = new();
		
		private int _currentCycleCount;
		private int _currentPresetNum;

		public BalanceAnalyzeProcess(IRobotController robotController,
			ILevelServiceOld<StageStaticData, LevelStaticData> levelServiceOld,
			IWindowsService windowsService,
			IBattleService<BattleContext> battleService,
			IArsenalService<WeaponStaticData> arsenalService,
			IPlayerEvents playerEvents,
			AnalyzeRecorder analyzeRecorder,
			FpsCounter fpsCounter,
			IBattleStatisticsService battleStatisticsService) : base(robotController)
		{
			_levelServiceOld = levelServiceOld;
			_windowsService = windowsService;
			_battleService = battleService;
			_arsenalService = arsenalService;
			_playerEvents = playerEvents;
			_fpsCounter = fpsCounter;
			_battleStatisticsService = battleStatisticsService;
		}

		protected override void RunInternal()
		{
			_analyzePresets.Clear();
			
			_currentPresetNum = 0;
			_currentCycleCount = 0;
			
			NewPreset();
			ApplyPreset();
			
			JumpToLevel(_configuration.StartLevelName);
			
			_robotController.SetActivity(true);

			_battleService.OnBattleBegin += OnBattleBegin;
			_battleService.OnBattleRestart += OnBattleBegin;
			_levelServiceOld.OnLevelCompleted += OnLevelCompleted;
			_playerEvents.OnPlayerDied += OnPlayerDied;
		}

		public override void Kill()
		{
			_robotController.SetActivity(false);

			_battleService.OnBattleBegin -= OnBattleBegin;
			_battleService.OnBattleRestart -= OnBattleBegin;
			_levelServiceOld.OnLevelCompleted -= OnLevelCompleted;
			_playerEvents.OnPlayerDied -= OnPlayerDied;
		}

		private void OnBattleBegin()
		{
			_fpsCounter.StartMeasurement();
		}

		private void OnPlayerDied()
		{
			_analyzePresets[_currentPresetNum].RecordPlayerDeath(_battleService.CurrentEnemyActiveGroupId);
		}

		private void OnLevelCompleted(string levelName)
		{
			RecordLevelCompleted();
			TryNextOrComplete(levelName);
		}

		private void RecordLevelCompleted()
		{
			var battleTime = Time.time - _battleService.LastBattleBeginTime; // NOTE: Сalculation is incorrect now
			var averageFps = _fpsCounter.StopMeasurement();
			var loadTime = _battleService.LastBattleLoadingTime;

			float earnedMoney = _battleStatisticsService.GetTotalBonus();
			_battleService.BattleContext.LevelSettings.MissionRewards
				.Where(r => r is CurrencyReward currencyReward && currencyReward.Currency.CurrencyType == CurrencyType.BattlePoints)
				.Cast<CurrencyReward>()
				.ForEach(r => earnedMoney += r.Currency.Value);

			float healthPercent = _playerEvents.HealthPercent;
			float primaryPercent = (float)_arsenalService.GetAmmoCount(_arsenalService.CurrentPrimaryWeapon) / _arsenalService.GetBaseAmmoCount(_arsenalService.CurrentPrimaryWeapon);
			float secondaryPercent = (float)_arsenalService.GetAmmoCount(_arsenalService.CurrentSecondaryWeapon) / _arsenalService.GetBaseAmmoCount(_arsenalService.CurrentSecondaryWeapon);

			_analyzePresets[_currentPresetNum].RecordLevelCompleted(healthPercent, primaryPercent, secondaryPercent, earnedMoney, battleTime, averageFps, loadTime);
		}

		private void CompletePreset()
		{
			_currentPresetNum++;
		}

		private void NewPreset()
		{
			_analyzePresets.Add(new AnalyzeRecorder(_battleService));
		}

		private void CompleteProcess()
		{
			_onComplete?.Invoke();
			
#if UNITY_EDITOR
			AnalyzeWriter.WriteSheet(_analyzePresets);
#endif
		}

		private void TryNextOrComplete(string levelName)
		{
			if (!_configuration.EndLevelName.Equals(levelName))
				return;

			if (_currentCycleCount >= _configuration.CycleCount - 1)
			{
				CompletePreset();

				if (_currentPresetNum >= _configuration.Presets.Count)
				{
					CompleteProcess();
					return;
				}

				_currentCycleCount = 0;
					
				NewPreset();
				ApplyPreset();
			}
			else
			{
				_currentCycleCount++;
			}

			JumpToLevel(_configuration.StartLevelName);
		}

		private void ApplyPreset()
		{
			var preset = _configuration.Presets[_currentPresetNum];
			
			_arsenalService.MarkWeaponAsPurchased(preset.PrimaryWeaponType);
			_arsenalService.MarkWeaponAsPurchased(preset.SecondaryWeaponType);
			
			_arsenalService.CurrentPrimaryWeapon = preset.PrimaryWeaponType;
			_arsenalService.CurrentSecondaryWeapon = preset.SecondaryWeaponType;
		}

		private void JumpToLevel(string levelName)
		{
			_levelServiceOld.JumpTo(levelName, true);

			if (_battleService.IsBattle)
				_battleService.EndBattle(BattleResult.Leave);
			else
				_windowsService.TryPlayWindowTransition(_windowsService.ActiveWindows.First(), WindowType.MapWindow);
		}
	}
}