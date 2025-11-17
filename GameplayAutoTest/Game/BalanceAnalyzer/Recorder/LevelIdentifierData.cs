using System;

namespace Swat.Game.GameplayRobot.Game.BalanceAnalyzer.Recorder
{
	public readonly struct LevelIdentifierData : IEquatable<LevelIdentifierData>
	{
		public int StageIndex { get; }
		public int LevelIndex { get; }

		public LevelIdentifierData(int stageIndex, int raidIndex, int levelIndex)
		{
			StageIndex = stageIndex;
			LevelIndex = levelIndex;
		}
		
		public LevelIdentifierData(int stageIndex, int levelIndex)
		{
			StageIndex = stageIndex;
			LevelIndex = levelIndex;
		}

		public bool Equals(LevelIdentifierData other)
		{
			return StageIndex == other.StageIndex
				   && LevelIndex == other.LevelIndex;
		}

		public override bool Equals(object obj)
		{
			return obj is LevelIdentifierData other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(StageIndex, LevelIndex);
		}
	}
}