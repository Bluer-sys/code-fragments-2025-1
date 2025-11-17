using Swat.Game.GameplayRobot.Core.Interfaces;
using Swat.Game.GameplayRobot.Core.Task;

namespace Swat.Game.GameplayRobot.Game.DefeatContext
{
	public class ReviveClickTask : RobotTask
	{
		public ReviveClickTask(IGameServiceAdapter game) : base(game)
		{
		}

		public override bool CanExecute()
		{
			return true;
		}

		public override void OnStart()
		{
			_game.ClickReviveOrRetry();
		}
		
		public override void Execute()
		{
			if (_game.IsHealingProcess())
				return;
			
			Complete();
		}
	}
}