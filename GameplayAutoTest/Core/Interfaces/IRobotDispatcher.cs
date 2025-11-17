using System;
using Swat.Game.GameplayRobot.Core.Data;

namespace Swat.Game.GameplayRobot.Core.Interfaces
{
	public interface IRobotDispatcher
	{
		void Launch(RobotConfiguration configuration, Action onProcessComplete = null);
		void Break();
	}
}