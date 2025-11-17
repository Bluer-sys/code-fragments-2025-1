using Swat.Game.GameplayRobot.Core.Interfaces;
using Swat.Game.GameplayRobot.Core.Task;
using Swat.Game.Utils;
using UnityEngine;

namespace Swat.Game.GameplayRobot.Game.GameContext
{
	public class TakeBestWeaponTask : RobotTask
	{
		private const float AfterWeaponChangeDelay = 0.8f;
		
		private readonly GameContextBlackboard _blackboard;
		
		private float _lastWeaponChangeTime;

		public TakeBestWeaponTask(IGameServiceAdapter game, GameContextBlackboard blackboard) : base(game)
		{
			_blackboard = blackboard;
		}

		public override bool CanExecute()
		{
			return _game.CanShootAtEnemies;
		}

		public override void OnStart()
		{
			if (_blackboard.EnemyTarget == null)
				return;
			
			if(_game.CalculateAndSetBestWeapon(_blackboard.EnemyTarget))
				_lastWeaponChangeTime = Time.time;
		}
		
		public override void Execute()
		{
			if(_blackboard.EnemyTarget == null)
				Complete();
			
			if(TimeUtils.IsTimeExpired(_lastWeaponChangeTime, AfterWeaponChangeDelay))
				Complete();
		}
	}
}