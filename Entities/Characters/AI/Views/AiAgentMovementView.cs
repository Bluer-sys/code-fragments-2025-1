using Swat.Game.Entities.Characters.Common.Views;
using Swat.Game.Entities.Core.Characters.AI.Views;
using UnityEngine;
using UnityEngine.AI;

namespace Swat.Game.Entities.Characters.AI.Views
{
	public class AiAgentMovementView : CharacterMovementView, IAiAgentMovementView
	{
		[field: SerializeField] public NavMeshAgent NavMeshAgent { get; private set; }


		public override bool IsMoving =>
			NavMeshAgent.hasPath
			&& NavMeshAgent.remainingDistance > NavMeshAgent.stoppingDistance
			&& NavMeshAgent.speed > 0;
	}
}