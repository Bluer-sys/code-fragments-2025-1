namespace Swat.Game.Entities.Characters.Player.ExecutionContexts
{
	public class PlayerSniperExecutionContext : PlayerBaseExecutionContext, ICharacterExecutionContext
	{
		public PlayerSniperExecutionContext(ICharacterEntity entity,
			ICharacterModel[] models,
			ICharacterView[] views,
			ICharacterController[] controllers)
			: base(entity, models, views, controllers)
		{
		}
	}
}