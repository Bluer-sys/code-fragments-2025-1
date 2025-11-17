using UnityEngine;

namespace Swat.Game.Entities.Characters.AI.Models
{
	public class AiEffectsModel : BaseModel, ICharacterModel
	{
		[field: SerializeField] public Vector2 BleedingTime { get; private set; }
	}
}