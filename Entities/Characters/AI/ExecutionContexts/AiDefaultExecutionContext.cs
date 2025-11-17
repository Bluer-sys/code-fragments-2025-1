namespace Swat.Game.Entities.Characters.AI.ExecutionContexts
{
	public class AiDefaultExecutionContext : AiBaseExecutionContext
	{
		public AiDefaultExecutionContext(ICharacterEntity entity,
			ICharacterModel[] models,
			ICharacterView[] views,
			ICharacterController[] controllers)
			: base(entity, models, views, controllers)
		{
		}
	}
}