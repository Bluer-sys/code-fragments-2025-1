using Swat.Game.Services.WindowsService.Core.Common;

namespace Swat.Game.UI.Windows.GameWindow.Core
{
	public interface IEnemyUiController : IUiController
	{
		void OnEnemyDeath();
	}
}