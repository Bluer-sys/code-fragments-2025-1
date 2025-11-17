using Swat.Game.Entities.Characters.Common.Data;
using UnityEngine;

namespace Swat.Game.Entities.Characters.AI
{
	[SelectionBase]
	public class AIEntity : CharacterEntity
	{
		public override CharacterType CharacterType => CharacterType.Enemy;
	}
}