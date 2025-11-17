using Swat.Game.Entities.Core.Characters.Common.Views;

namespace Swat.Game.Entities.Characters.Common.Views
{
	public abstract class CharacterMovementView : IBaseMovementView
	{
		public abstract bool IsMoving { get; }

		public void Refresh()
		{
		}
	}
}