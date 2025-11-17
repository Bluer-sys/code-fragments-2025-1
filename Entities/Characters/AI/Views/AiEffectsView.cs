using Swat.Game.Entities.Core.Characters.AI.Views;
using Swat.Game.Services.VfxService;
using Swat.Utils;
using UnityEngine;

namespace Swat.Game.Entities.Characters.AI.Views
{
	public class AiEffectsView : IAiEffectsView
	{
		[field: SerializeField] public SerializableDictionary<VfxType, ParticleSystem> Vfxs { get; set; }

		public void Refresh()
		{
		}
	}
}