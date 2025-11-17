using Swat.Game.Services.WindowsService;
using Swat.UI;
using Swat.Utils;
using UnityEngine;

namespace Swat.Game.UI.Windows.GameWindow.Elements.TwoFingersControls
{
	public class TwoFingersControlsUi : BaseWindow
	{
		[field: SerializeField] public InputHandler ShootButton { get; private set; }
		[field: SerializeField] public SwappableButton CoverButton { get; private set; }
		[field: SerializeField] public SwappableButton ScopeButton { get; private set; }
		[field: SerializeField] public JoyPad Joystick { get; private set; }

		[field: SerializeField] public SerializableDictionary<ButtonState, GameObject> ScopeButtonStates { get; private set; }
	}
}