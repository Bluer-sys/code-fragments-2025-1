using Swat.Game.Entities.Core.Characters.AI.Models;
using UnityEngine;

namespace Swat.Game.Entities.Characters.AI.Models
{
	public class AiMortalShootModel : BaseModel, IAiMortalShootModel
	{
		[field: SerializeField] public float MortalDamageMultiplier { get; private set; }
		[field: SerializeField] public Vector3 LensFlareOffset { get; private set; }
	}
}