using Swat.Game.GameplayRobot.Core.Interfaces;
using Swat.Game.GameplayRobot.Core.Task;

namespace Swat.Game.GameplayRobot.Game.GameContext
{
	public class FindPressurePointTask : RobotTask
	{
		private readonly GameContextBlackboard _blackboard;
		
		public FindPressurePointTask(IGameServiceAdapter game, GameContextBlackboard blackboard) : base(game)
		{
			_blackboard = blackboard;
		}

		public override bool CanExecute()
		{
			return _game.CanShootAtEnemies;
		}

		public override void Execute()
		{
			if(_blackboard.EnemyTarget != null)
				_blackboard.EnemyShootTarget = _game.CalculateBestShootTarget(_blackboard.EnemyTarget);
			
			Complete();
		}
	}
}