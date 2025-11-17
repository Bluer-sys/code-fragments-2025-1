using Swat.Game.Entities.Core.Characters.AI.Models;
using UnityEngine;

namespace Swat.Game.Entities.Characters.AI.Models
{
	public class AiShieldModel : BaseModel, IAiShieldModel
	{
		[field: SerializeField] public string DropShieldCurveParameterName { get; private set; }

	}
}