using System;
using Swat.Game.GameplayRobot.Core.Data;

namespace Swat.Game.GameplayRobot.Core.Process
{
	public interface IRobotProcess
	{
		Type ConfigurationType { get; }
		
		void Run(RobotConfiguration configuration, Action onComplete = null);
		void Kill();
	}
}