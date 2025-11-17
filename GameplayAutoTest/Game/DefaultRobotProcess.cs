using Swat.Game.GameplayRobot.Core.Data;
using Swat.Game.GameplayRobot.Core.Interfaces;
using Swat.Game.GameplayRobot.Core.Process;

namespace Swat.Game.GameplayRobot.Game
{
	public class DefaultRobotProcess : RobotProcess<RobotConfiguration>
	{
		public DefaultRobotProcess(IRobotController robotController) : base(robotController) { }

		protected override void RunInternal()
		{
			_robotController.SetActivity(true);
		}

		public override void Kill()
		{
			_robotController.SetActivity(false);
		}
	}
}