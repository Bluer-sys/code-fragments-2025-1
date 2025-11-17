using Swat.Game.GameplayRobot.Core.Context;
using Swat.Game.GameplayRobot.Game.Common;

namespace Swat.Game.GameplayRobot.Game.VictoryContext
{
	public class VictoryRobotContext : RobotContext
	{
		public override bool IsLooped => false;
		
		protected override void RegisterTasks()
		{
			AddTask(new WaitTask(_game, 1f));
			AddTask(new GetRewardAndContinueTask(_game));
		}
	}
}