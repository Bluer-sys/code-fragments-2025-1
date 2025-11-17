using Swat.Game.Entities.Characters.AI.Models;
using Swat.Game.Entities.Core.Characters.AI.Controllers;

namespace Swat.Game.Entities.Characters.AI.Controllers
{
	public class AiGroupMemberController : BaseController, IAiGroupMemberController
	{
		private readonly AiGroupMemberModel _model;

		public AiGroupMemberController(AiGroupMemberModel model)
		{
			_model = model;
		}


		public void ChangeGroup(int groupId)
		{
			_model.GroupId = groupId;
		}

		public bool IsMyGroupId(int id)
		{
			return _model.GroupId == id;
		}
	}
}