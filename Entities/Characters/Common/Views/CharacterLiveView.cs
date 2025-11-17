using System;
using Swat.Game.Entities.Core;
using Swat.Game.Entities.Core.Characters.Common.Views;

namespace Swat.Game.Entities.Characters.Common.Views
{
	public class CharacterLiveView : BaseLiveView, ICharacterLiveView
	{
		public event Action OnAnyHitReceived;


		public void Refresh()
		{
		}


		public void ReceiveAnyHit()
		{
			OnAnyHitReceived?.Invoke();
		}
	}
}