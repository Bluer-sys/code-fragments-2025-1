using Swat.Game.GameplayRobot.Core.Interfaces;
using Swat.Game.GameplayRobot.Core.Task;

namespace Swat.Game.GameplayRobot.Game.GameContext
{
	public class TryUseMedKitTask : RobotTask
	{
		private const float HealthPercentThreshold = 0.15f;

		public TryUseMedKitTask(IGameServiceAdapter game) : base(game)
		{
		}

		public override bool CanExecute()
		{
			return true;
		}

		public override void OnStart()
		{
			if(_game.PlayerHealthPercent < HealthPercentThreshold)
				_game.TryUseMedKit();
		}

		public override void Execute()
		{
			if(_game.IsHealingProcess())
				return;
			
			Complete();
		}
	}
}