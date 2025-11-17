namespace Swat.Game.GameplayRobot.Core.Task
{
	public interface IRobotTask
	{
		bool IsCompleted { get; }
		bool CanExecute();
		void OnStart();
		void Execute();
		void Reset();
	}
}