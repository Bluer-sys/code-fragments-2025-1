using System;
using Swat.Game.GameplayRobot.Core.Data;
using Swat.Game.GameplayRobot.Core.Interfaces;
using Zenject;

namespace Swat.Game.GameplayRobot.Core.Process
{
	public abstract class RobotProcess<T> : IRobotProcess, IInitializable, ITickable, IDisposable
		where T : RobotConfiguration
	{
		protected readonly IRobotController _robotController;
		
		protected T _configuration;
		protected Action _onComplete;

		protected RobotProcess(IRobotController robotController)
		{
			_robotController = robotController;
		}

		public Type ConfigurationType => typeof(T);
		
		public void Run(RobotConfiguration configuration, Action onComplete = null)
		{
			if (configuration != null && configuration is not T)
				throw new ArgumentException($"Configuration type must be convertible to {ConfigurationType}", nameof(configuration));
			
			_configuration = (T)configuration;
			_onComplete = onComplete;
			
			RunInternal();
		}

		protected abstract void RunInternal();

		public virtual void Kill() { }
		
		
		public virtual void Initialize() { }

		public virtual void Tick() { }

		public virtual void Dispose() { }
	}
}