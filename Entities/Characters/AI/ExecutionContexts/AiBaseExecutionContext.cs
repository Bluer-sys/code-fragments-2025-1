namespace Swat.Game.Entities.Characters.AI.ExecutionContexts
{
	public abstract class AiBaseExecutionContext :
		BaseExecutionContext<ICharacterModel, ICharacterView, ICharacterController>, ICharacterExecutionContext
	{
		protected AiBaseExecutionContext(ICharacterEntity entity,
			ICharacterModel[] models,
			ICharacterView[] views,
			ICharacterController[] controllers)
			: base(entity, models, views, controllers)
		{
		}
	}
}