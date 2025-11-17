using Swat.Game.GameplayRobot.Core.Context;
using Swat.Game.GameplayRobot.Game.Common;

namespace Swat.Game.GameplayRobot.Game.ArsenalContext
{
	public class ArsenalRobotContext : RobotContext
	{
		public override bool IsLooped => false;
		
		protected override void RegisterTasks()
		{
			AddTask(new WaitTask(_game, 2f));
			AddTask(new UpgradeCurrentWeaponWhileHasCurrencyTask(_game));
			AddTask(new StartLevelClickTask(_game));
		}
	}
}