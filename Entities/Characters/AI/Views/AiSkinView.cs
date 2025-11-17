using Swat.Game.Entities.Core.Characters.AI.Views;
using UnityEngine;

namespace Swat.Game.Entities.Characters.AI.Views
{
	public class AiSkinView : IAiSkinView
	{
		[field: SerializeField] public SkinnedMeshRenderer Head0Renderer { get; private set; }
		[field: SerializeField] public SkinnedMeshRenderer Head1Renderer { get; private set; }
		[field: SerializeField] public SkinnedMeshRenderer BodyRenderer { get; private set; }
		[field: SerializeField] public SkinnedMeshRenderer GlovsRenderer { get; private set; }
		[field: SerializeField] public SkinnedMeshRenderer PantsRenderer { get; private set; }
		[field: SerializeField] public ProjectorForLWRP.ProjectorForLWRP Projector  { get; private set; }

		public void Refresh()
		{
		}
	}
}