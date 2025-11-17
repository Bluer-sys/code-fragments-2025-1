namespace Swat.Game.Entities.Characters.AI.ExecutionContexts
{
	public class AiCarExecutionContext : AiBaseExecutionContext
	{
		public AiCarExecutionContext(ICharacterEntity entity,
			ICharacterModel[] models,
			ICharacterView[] views,
			ICharacterController[] controllers)
			: base(entity, models, views, controllers)
		{
		}
	}
}