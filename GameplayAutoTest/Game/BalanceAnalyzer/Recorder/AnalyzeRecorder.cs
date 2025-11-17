using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Swat.Game.Services.BattleService;

namespace Swat.Game.GameplayRobot.Game.BalanceAnalyzer.Recorder
{
	[UsedImplicitly]
	public class AnalyzeRecorder
	{
		private readonly IBattleService<BattleContext> _battleService;

		private readonly Dictionary<LevelIdentifierData, LevelAnalyzeRecord> _levelRecords = new();
		
		public IReadOnlyList<LevelAnalyzeRecord> LevelRecords => _levelRecords.Values.ToList();
		
		public AnalyzeRecorder(IBattleService<BattleContext> battleService)
		{
			_battleService = battleService;
		}
		
		public void RecordPlayerDeath(int segmentIndex)
		{
			var record = CreateOrGetCurrentRecord();

			record.LooseCount++;
			record.LooseCountSegments[segmentIndex]++;
		}

		public void RecordLevelCompleted(float healthPercent, float primaryPercent, float secondaryPercent, float earnedMoney, float playTime, float averageFps, float loadTime)
		{
			var record = CreateOrGetCurrentRecord();
			
			record.RemainingHealthPercent.Add(healthPercent);
			record.RemainingAmmoPrimaryPercent.Add(primaryPercent);
			record.RemainingAmmoSecondaryPercent.Add(secondaryPercent);
			record.BattleEarnMoney.Add(earnedMoney);
			record.PlayTimes.Add(playTime);
			record.AverageFps.Add(averageFps);
			record.SceneLoadTime.Add(loadTime);
		}

		public void Reset()
		{
			_levelRecords.Clear();
		}

		private LevelAnalyzeRecord CreateOrGetCurrentRecord()
		{
			var curLevelIdentifierData = GetCurrentLevelRecordData();
			return CreateOrGetRecord(curLevelIdentifierData);
		}

		private LevelAnalyzeRecord CreateOrGetRecord(LevelIdentifierData levelIdentifierData)
		{
			if (!_levelRecords.ContainsKey(levelIdentifierData))
			{
				_levelRecords.Add(levelIdentifierData, new LevelAnalyzeRecord
				{
					StageIndex = levelIdentifierData.StageIndex,
					LevelIndex = levelIdentifierData.LevelIndex
				});
			}
			
			return _levelRecords[levelIdentifierData];
		}

		private LevelIdentifierData GetCurrentLevelRecordData()
		{
			return new LevelIdentifierData(_battleService.BattleContext.StageIndex, _battleService.BattleContext.LevelIndex);
		}
	}
}