using Swat.Game.GameplayRobot.Core.Interfaces;

namespace Swat.Game.GameplayRobot.Core.Context
{
	public interface IRobotContext
	{
		void Initialize(IGameServiceAdapter service);
		void ExecuteLogic();
		void OnEnter();
		void OnExit();
		void Reset();
	}
}