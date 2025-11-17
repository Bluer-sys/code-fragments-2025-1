using System;
using UnityEngine;

namespace Swat.Game.UI.Windows.GameWindow.Core
{
	public interface ITwoFingersControlsUiController
	{
		event Action OnShootButtonDown;
		event Action OnShootButtonUp;
		event Action OnCoverButtonDown;
		event Action OnScopeButtonDown;
		event Action OnJoystickDrag;

		Vector2 JoystickDirection { get; }
		Vector2 JoystickDelta { get; }

		bool IsJoystickButtonPressed { get; }
		bool IsShootButtonPressed { get; }


		void Reset();
	}
}