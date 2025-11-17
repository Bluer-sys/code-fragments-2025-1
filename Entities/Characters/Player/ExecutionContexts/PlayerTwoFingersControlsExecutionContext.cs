namespace Swat.Game.Entities.Characters.Player.ExecutionContexts
{
	public class PlayerTwoFingersControlsExecutionContext : PlayerBaseExecutionContext, ICharacterExecutionContext
	{
		public PlayerTwoFingersControlsExecutionContext(ICharacterEntity entity,
			ICharacterModel[] models,
			ICharacterView[] views,
			ICharacterController[] controllers)
			: base(entity, models, views, controllers)
		{
		}
	}
}