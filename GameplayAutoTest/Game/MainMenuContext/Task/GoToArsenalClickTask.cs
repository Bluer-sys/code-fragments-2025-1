using Swat.Game.GameplayRobot.Core.Interfaces;
using Swat.Game.GameplayRobot.Core.Task;

namespace Swat.Game.GameplayRobot.Game.MainMenuContext.Task
{
	public class GoToArsenalClickTask : RobotTask
	{
		public GoToArsenalClickTask(IGameServiceAdapter game) : base(game)
		{
		}

		public override bool CanExecute()
		{
			return true;
		}

		public override void OnStart()
		{
			_game.ClickGoToArsenal();
		}
		
		public override void Execute()
		{
			Complete();
		}
	}
}