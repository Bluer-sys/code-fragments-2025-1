using System;
using Swat.Game.Services.WindowsService;

namespace Swat.Game.UI.Windows.GameWindow.Core
{
	public interface IGameplayUiController
	{
		event Action<UiOrientation> OnChangeGameplayOrientation;
		event Action OnSettingsButtonClicked;


		UiOrientation CurrentGameplayOrientation { get; }


		void OnMissionComplete();

		void OnMissionFailed();
		
		void SetVisible(bool isVisible);
	}
}