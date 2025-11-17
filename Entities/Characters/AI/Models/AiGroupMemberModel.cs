using Swat.Game.Entities.Core.Characters.AI.Models;

namespace Swat.Game.Entities.Characters.AI.Models
{
	public class AiGroupMemberModel : BaseModel, IAiGroupMemberModel
	{
		public int GroupId { get; set; }
	}
}