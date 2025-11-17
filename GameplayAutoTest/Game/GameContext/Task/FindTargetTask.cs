using Swat.Game.GameplayRobot.Core.Interfaces;
using Swat.Game.GameplayRobot.Core.Task;

namespace Swat.Game.GameplayRobot.Game.GameContext
{
	public class FindTargetTask : RobotTask
	{
		private readonly GameContextBlackboard _blackboard;
		
		public FindTargetTask(IGameServiceAdapter game, GameContextBlackboard blackboard) : base(game)
		{
			_blackboard = blackboard;
		}

		public override bool CanExecute()
		{
			return _game.CanShootAtEnemies;
		}

		public override void OnStart()
		{
			_blackboard.EnvironmentTarget = null;
			_blackboard.EnemyTarget = null;
		}

		public override void Execute()
		{
			_blackboard.EnvironmentTarget = _game.FindEnvironmentTarget();
			
			if (_blackboard.EnvironmentTarget == null)
				_blackboard.EnemyTarget = _game.FindTarget();
			
			Complete();
		}
	}
}