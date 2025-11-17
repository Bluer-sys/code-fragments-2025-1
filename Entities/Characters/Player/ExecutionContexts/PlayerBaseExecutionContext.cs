namespace Swat.Game.Entities.Characters.Player.ExecutionContexts
{
	public abstract class
		PlayerBaseExecutionContext : BaseExecutionContext<ICharacterModel, ICharacterView, ICharacterController>
	{
		protected PlayerBaseExecutionContext(ICharacterEntity entity,
			ICharacterModel[] models,
			ICharacterView[] views,
			ICharacterController[] controllers)
			: base(entity, models, views, controllers)
		{
		}
	}
}