using Swat.Game.GameplayRobot.Core.Interfaces;
using Swat.Game.GameplayRobot.Core.Task;

namespace Swat.Game.GameplayRobot.Game.GameContext
{
	public class EndAimTask : RobotTask
	{
		public EndAimTask(IGameServiceAdapter game) : base(game)
		{
		}

		public override bool CanExecute()
		{
			return true;
		}

		public override void OnStart()
		{
			_game.SetAutoAimLock(false);
			_game.ExitAimState();
		}

		public override void Execute()
		{
			Complete();
		}
	}
}