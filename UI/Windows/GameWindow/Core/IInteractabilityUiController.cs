using System;

namespace Swat.Game.UI.Windows.GameWindow.Core
{
	public interface IInteractabilityUiController
	{
		void SetUiInteractable(bool isInteractable);
		void SetUiVisible(bool visible);
		
		event Action<bool> OnUiInteractableChanged;
	}
}