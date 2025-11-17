using System;
using Swat.Game.GameControllers.EntitySpawners.Data;
using Swat.Game.GameplayRobot.Core.Interfaces;
using Swat.Game.GameplayRobot.Core.Task;
using Swat.Game.Utils;
using UnityEngine;

namespace Swat.Game.GameplayRobot.Game.GameContext
{
	public class ShootTargetTask : RobotTask
	{
		private readonly GameContextBlackboard _blackboard;
		
		private const float FindTargetDelay = 0.01f;
		private const int MaxFindTargetTimes = 1000;
		
		private const float EnvironmentPosHeightOffset = 0.3f;

		private float _lastFindTargetTime;
		private int _findTargetCounter;

		private Func<bool> IsEntityDead { get; set; }
		private Func<Vector3> ShootPosition { get; set; }

		public ShootTargetTask(IGameServiceAdapter game, GameContextBlackboard blackboard) : base(game)
		{
			_blackboard = blackboard;
		}

		public override bool CanExecute()
		{
			return _game.CanShootAtEnemies && !CanCompleteByTooManyEnemiesShooting();
		}

		public override void OnStart()
		{
			if (_blackboard.EnvironmentTarget != null)
			{
				IsEntityDead = () => RobotUtils.IsEntityDead(_blackboard.EnvironmentTarget);
				ShootPosition = () => _blackboard.EnvironmentTarget.Transform.position + Vector3.up * EnvironmentPosHeightOffset;
			}
			else if (_blackboard.EnemyTarget != null)
			{
				IsEntityDead = () => RobotUtils.IsEntityDead(_blackboard.EnemyTarget);
				ShootPosition = () => _blackboard.EnemyShootTarget.position;
			}
			else
			{
				IsEntityDead = () => true;
				ShootPosition = () => Vector3.zero;
			}
		}

		public override void Execute()
		{
			if(IsEntityDead())
				Complete();

			if (CanCompleteByTooManyEnemiesShooting())
				Complete();
			
			if (!TimeUtils.IsTimeExpired(_lastFindTargetTime, FindTargetDelay))
				return;

			_lastFindTargetTime = Time.time;
			
			_game.AimAtTarget(ShootPosition());
				
			if (++_findTargetCounter >= MaxFindTargetTimes)
				Complete();
		}

		private bool CanCompleteByTooManyEnemiesShooting() 
		{
			return _game.ShootEnemiesNowCount() > 1 && !_game.IsMeaningfulTarget(_blackboard.EnemyTarget);
		}

		public override void Reset()
		{
			base.Reset();
			
			_lastFindTargetTime = 0;
			_findTargetCounter = 0;
			
			_game.SetAutoAimLock(false);
		}
	}
}