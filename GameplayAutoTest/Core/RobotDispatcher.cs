using System;
using System.Linq;
using Swat.Game.GameplayRobot.Core.Data;
using Swat.Game.GameplayRobot.Core.Interfaces;
using Swat.Game.GameplayRobot.Core.Process;
using Swat.Game.GameplayRobot.Game;

namespace Swat.Game.GameplayRobot.Core
{
	public class RobotDispatcher : IRobotDispatcher
	{
		private readonly IRobotProcess[] _robotProcesses;

		private IRobotProcess _currentProcess;

		public RobotDispatcher(IRobotProcess[] robotProcesses)
		{
			_robotProcesses = robotProcesses;
		}
		
		public void Launch(RobotConfiguration configuration, Action onProcessComplete = null)
		{
			_currentProcess = GetProcess(configuration);
			_currentProcess.Run(configuration, onProcessComplete);
		}

		public void Break()
		{
			_currentProcess?.Kill();
			_currentProcess = null;
		}

		private IRobotProcess GetProcess(RobotConfiguration configuration)
		{
			return configuration != null 
				? _robotProcesses.FirstOrDefault(p => p.ConfigurationType == configuration.GetType()) 
				: _robotProcesses.FirstOrDefault(p => p.GetType() == typeof(DefaultRobotProcess));
		}
	}
}