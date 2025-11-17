using Swat.Game.Entities.Characters.Common.Views;
using Swat.Game.Entities.Core;
using Swat.Game.Entities.Core.Characters.AI.Views;
using UnityEngine;

namespace Swat.Game.Entities.Characters.AI.Views
{
	public class AiLiveView : CharacterLiveView, IAiLiveView
	{
		[field: SerializeField] public GameObject AnimationRoot { get; private set; }

		public void Refresh()
		{
		}
	}
}