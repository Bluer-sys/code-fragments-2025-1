using Swat.Game.GameplayRobot.Core.Interfaces;

namespace Swat.Game.GameplayRobot.Core.Task
{
	public abstract class RobotTask : IRobotTask
	{
		protected readonly IGameServiceAdapter _game;

		public bool IsCompleted { get; private set; }

		protected RobotTask(IGameServiceAdapter game)
		{
			_game = game;
		}

		public abstract bool CanExecute();

		public virtual void OnStart()
		{
		}

		public virtual void Execute()
		{
		}

		public virtual void Reset()
		{
			IsCompleted = false;
		}

		protected void Complete() 
		{
			IsCompleted = true;
		}
	}
}