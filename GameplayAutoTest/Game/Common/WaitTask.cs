using Swat.Game.GameplayRobot.Core.Interfaces;
using Swat.Game.GameplayRobot.Core.Task;
using Swat.Game.Utils;
using UnityEngine;

namespace Swat.Game.GameplayRobot.Game.Common
{
	public class WaitTask : RobotTask, IRobotTask
	{
		private readonly float _waitTime;
		private float? _beginWaitTime;	
		
		public WaitTask(IGameServiceAdapter game, float waitTime) : base(game)
		{
			_waitTime = waitTime;
		}

		public override bool CanExecute()
		{
			return true;
		}

		public override void Execute()
		{
			_beginWaitTime ??= Time.time;

			if (!TimeUtils.IsTimeExpired(_beginWaitTime.Value, _waitTime))
				return;
			
			Complete();
		}

		public override void Reset()
		{
			base.Reset();

			_beginWaitTime = null;
		}
	}
}