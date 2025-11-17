using System;
using Swat.Game.Services.WindowsService;
using Swat.Game.UI.Windows.GameWindow.Core;
using UnityEngine;

namespace Swat.Game.UI.Windows.GameWindow.Elements.Input
{
	[Serializable]
	public class InputHandlerUiController : BaseUiController<InputHandlerWindow>, IPointerUiController
	{
		public event Action<Vector2> OnPointerDrag;
		public event Action<Vector2> OnPointerBeginDrag;
		public event Action<Vector2> OnPointerEndDrag;
		public event Action<Vector2> OnPointerMove;
		public event Action OnPointerUpEvent;
		public event Action OnPointerDownEvent;


		public bool IsPointerHold
		{
			get => isPointerHoldLocked;
			private set
			{
				isPointerHold = value;

				if (!lockPointerHold)
					isPointerHoldLocked = isPointerHold;
			}
		}

		private bool isPointerHold;
		private bool isPointerHoldLocked;
		private bool lockPointerHold;


		public override void OnHideEnd()
		{
			base.OnHideEnd();

			OnPointerUp();
		}


		public override void RefreshViewBeforeChangeOrientation()
		{
			base.RefreshViewBeforeChangeOrientation();

			view.InputHandler.OnPointerDrag -= OnPointerDragEvent;
			view.InputHandler.OnPointerBeginDrag -= OnPointerBeginDragEvent;
			view.InputHandler.OnPointerEndDrag -= OnPointerEndDragEvent;
			view.InputHandler.OnPointerDownEvent -= OnPointerDown;
			view.InputHandler.OnPointerUpEvent -= OnPointerUp;
			view.InputHandler.OnPointerMoveEvent -= OnPointerMoveEvent;
		}


		public override void RefreshViewAfterChangeOrientation()
		{
			base.RefreshViewAfterChangeOrientation();

			view.InputHandler.OnPointerDrag += OnPointerDragEvent;
			view.InputHandler.OnPointerBeginDrag += OnPointerBeginDragEvent;
			view.InputHandler.OnPointerEndDrag += OnPointerEndDragEvent;
			view.InputHandler.OnPointerDownEvent += OnPointerDown;
			view.InputHandler.OnPointerUpEvent += OnPointerUp;
			view.InputHandler.OnPointerMoveEvent += OnPointerMoveEvent;
		}


		public void LockPointerHold(bool value)
		{
			lockPointerHold = true;
			isPointerHoldLocked = value;
		}

		public void UnlockPointerHold()
		{
			lockPointerHold = false;
			isPointerHoldLocked = isPointerHold;
		}

		private void OnPointerDragEvent(Vector2 delta)
		{
			OnPointerDrag?.Invoke(delta);
		}

		private void OnPointerBeginDragEvent(Vector2 delta)
		{
			OnPointerBeginDrag?.Invoke(delta);
		}

		private void OnPointerEndDragEvent(Vector2 delta)
		{
			OnPointerEndDrag?.Invoke(delta);
		}

		private void OnPointerMoveEvent(Vector2 delta)
		{
			OnPointerMove?.Invoke(delta);
		}

		private void OnPointerDown()
		{
			IsPointerHold = true;
			OnPointerDownEvent?.Invoke();
		}

		private void OnPointerUp()
		{
			IsPointerHold = false;
			OnPointerUpEvent?.Invoke();
		}
	}
}
