using Sirenix.OdinInspector;
using Swat.Game.Entities.Core.Characters.AI.Models;
using UnityEngine;

namespace Swat.Game.Entities.Characters.AI.Models
{
	public class AiShootModel : BaseModel, IAiShootModel
	{
		[field: SerializeField]
		[field: MinMaxSlider(0, 20f, true)]
		public Vector2 ShootDelay { get; private set; }

		[field: SerializeField]
		[field: MinMaxSlider(0, 20f, true)]
		public Vector2 DamageAimDispersion { get; private set; }

		[field: SerializeField]
		[field: MinMaxSlider(0, 20f, true)]
		public Vector2 FirstShootDelay { get; private set; }

		[field: SerializeField]
		[field: MinMaxSlider(0, 1f, true)]
		public Vector2 ClipCountProbability { get; private set; }


		public bool IsShootAvailable { get; set; }
		public bool IsReloadingProcessActive { get; set; }
		public bool IsShootNow { get; set; }
	}
}