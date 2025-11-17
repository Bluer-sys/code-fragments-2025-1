namespace Swat.Game.Entities.Characters.Player.ExecutionContexts
{
	public class PlayerDefaultExecutionContext : PlayerBaseExecutionContext, ICharacterExecutionContext
	{
		public PlayerDefaultExecutionContext(ICharacterEntity entity,
			ICharacterModel[] models,
			ICharacterView[] views,
			ICharacterController[] controllers)
			: base(entity, models, views, controllers)
		{
		}
	}
}