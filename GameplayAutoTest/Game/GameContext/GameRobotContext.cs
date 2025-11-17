using Swat.Game.GameplayRobot.Core.Context;
using Swat.Game.GameplayRobot.Game.Common;

namespace Swat.Game.GameplayRobot.Game.GameContext
{
	public class GameRobotContext : RobotContext
	{
		private const float AfterEndShootDelay = 0.5f;
		
		public override bool IsLooped => true;

		protected override void RegisterTasks()
		{
			var blackboard = new GameContextBlackboard();

			AddTask(new FindTargetTask(_game, blackboard));
			AddTask(new TakeBestWeaponTask(_game, blackboard));
			AddTask(new FindPressurePointTask(_game, blackboard));
			AddTask(new BeginAimTask(_game));
			AddTask(new ShootTargetTask(_game, blackboard));
			AddTask(new EndAimTask(_game));
			AddTask(new TryUseMedKitTask(_game));
			AddTask(new WaitTask(_game, AfterEndShootDelay));
		}

		public override void OnEnter()
		{
			base.OnEnter();

			_game.UpgradeWeaponsToCurrentLevelPower();
		}
	}
}