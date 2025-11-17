using System.Collections.Generic;

namespace Swat.Game.GameplayRobot.Game.BalanceAnalyzer.Recorder
{
	public class LevelAnalyzeRecord
	{
		public int StageIndex { get; set; }
		public int RaidIndex { get; set; }
		public int LevelIndex { get; set; }
		
		public List<float> RemainingHealthPercent { get; } = new();
		public List<float> RemainingAmmoPrimaryPercent { get; } = new();
		public List<float> RemainingAmmoSecondaryPercent { get; } = new();
		public List<float> BattleEarnMoney { get; } = new();
		
		public int LooseCount { get; set; }
		public int[] LooseCountSegments { get; } = new int[6];

		public List<float> PlayTimes { get; } = new();
		public List<float> AverageFps { get; } = new();
		public List<float> SceneLoadTime { get; } = new();
	}
}