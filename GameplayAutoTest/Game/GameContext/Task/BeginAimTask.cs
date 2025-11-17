using Swat.Game.GameplayRobot.Core.Interfaces;
using Swat.Game.GameplayRobot.Core.Task;

namespace Swat.Game.GameplayRobot.Game.GameContext
{
	public class BeginAimTask : RobotTask
	{
		public BeginAimTask(IGameServiceAdapter game) : base(game) {}

		public override bool CanExecute()
		{
			return true;
		}

		public override void OnStart()
		{
			if(!_game.CanAimAtEnemies)
				return;
			
			if(_game.ShootEnemiesNowCount() > 1)
				return;
			
			_game.SetAutoAimLock(true);
			_game.EnterAimState();
		}

		public override void Execute()
		{
			Complete();
		}
	}
}