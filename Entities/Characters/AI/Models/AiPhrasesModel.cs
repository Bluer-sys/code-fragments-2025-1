using Swat.Game.Entities.Core.Characters.AI.Models;
using UnityEngine;

namespace Swat.Game.Entities.Characters.AI.Models
{
	public class AiPhrasesModel : BaseModel, IAiPhrasesModel
	{
		public Vector2 PhraseDelayMinMax { get; set; }
	}
}