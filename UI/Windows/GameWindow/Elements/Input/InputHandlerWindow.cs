using Swat.Game.Services.WindowsService;
using UnityEngine;

namespace Swat.Game.UI.Windows.GameWindow.Elements.Input
{
	public class InputHandlerWindow : BaseWindow
	{
		[field: SerializeField] public InputHandler InputHandler { get; private set; }
	}
}