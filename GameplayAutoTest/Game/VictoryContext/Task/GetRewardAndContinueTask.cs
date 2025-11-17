using Swat.Game.GameplayRobot.Core.Interfaces;
using Swat.Game.GameplayRobot.Core.Task;

namespace Swat.Game.GameplayRobot.Game.VictoryContext
{
	public class GetRewardAndContinueTask : RobotTask
	{
		public GetRewardAndContinueTask(IGameServiceAdapter game) : base(game)
		{
		}

		public override bool CanExecute()
		{
			return true;
		}

		public override void OnStart()
		{
			_game.GetRewardAndContinue();
			_game.ResetHealthAmmoAndBattlePoints();
		}

		public override void Execute()
		{
			Complete();
		}
	}
}