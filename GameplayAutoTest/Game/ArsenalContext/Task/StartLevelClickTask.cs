using Swat.Game.GameplayRobot.Core.Interfaces;
using Swat.Game.GameplayRobot.Core.Task;

namespace Swat.Game.GameplayRobot.Game.ArsenalContext
{
	public class StartLevelClickTask : RobotTask
	{
		public StartLevelClickTask(IGameServiceAdapter game) : base(game)
		{
		}

		public override bool CanExecute()
		{
			return true;
		}

		public override void OnStart()
		{
			_game.ClickStartLevel();
		}
		
		public override void Execute()
		{
			Complete();
		}
	}
}