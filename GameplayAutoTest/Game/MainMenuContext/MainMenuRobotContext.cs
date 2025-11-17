using Swat.Game.GameplayRobot.Core.Context;
using Swat.Game.GameplayRobot.Game.Common;
using Swat.Game.GameplayRobot.Game.MainMenuContext.Task;

namespace Swat.Game.GameplayRobot.Game.MainMenuContext
{
	public class MainMenuRobotContext : RobotContext
	{
		private GoToArsenalClickTask _goToArsenalClickTask;
		
		public override bool IsLooped => false;
		
		public override void OnEnter()
		{
			base.OnEnter();

			_goToArsenalClickTask.Reset();
		}

		protected override void RegisterTasks()
		{
			_goToArsenalClickTask = new GoToArsenalClickTask(_game);
			
			AddTask(new WaitTask(_game, 1f));
			AddTask(_goToArsenalClickTask);
		}
	}
}