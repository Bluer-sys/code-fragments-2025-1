using System;
using Swat.Game.Services.WindowsService.Core.Common;
using UnityEngine;

namespace Swat.Game.UI.Windows.GameWindow.Core
{
	public interface IPointerUiController : IUiController
	{
		event Action<Vector2> OnPointerDrag;
		event Action<Vector2> OnPointerBeginDrag;
		event Action<Vector2> OnPointerEndDrag;
		event Action<Vector2> OnPointerMove;
		event Action OnPointerDownEvent;
		event Action OnPointerUpEvent;


		bool IsPointerHold { get; }


		void LockPointerHold(bool value);

		void UnlockPointerHold();
	}
}