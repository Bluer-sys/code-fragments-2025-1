using Swat.Game.BehaviourTree;
using Swat.Game.BehaviourTree.Blackboard;
using Swat.Game.BehaviourTree.Game.Contexts;
using Swat.Game.BehaviourTree.Interfaces;
using Swat.Game.Data.AI;
using Swat.Game.Entities.Bullets;
using Swat.Game.Entities.Core.Characters.AI.Controllers;
using Swat.Game.Entities.Core.Characters.Common.Controllers;
using Swat.Game.GameControllers;
using Swat.Game.GameControllers.EntitySpawners.Data;
using Swat.Game.Services.BattleService;
using Swat.Game.UI.Windows.PreGameWindow.Core;
using Swat.Game.UI.Windows.PreGameWindow.Elements.TapToPlay;
using Swat.Utils;
using UnityEngine;
using Zenject;

namespace Swat.Game.Entities.Characters.AI.Controllers
{
	public class AiBehaviourController : BaseController, IAiBehaviourController
	{
		protected readonly ICharacterEntity _entity;
		protected readonly ICharacterSpawnContext _spawnContext;
		private readonly IBehaviourTreeManager _behaviourTreeManager;
		protected readonly IBattleService<BattleContext> _battleService;
		private readonly IInstantiator _instantiator;
		private readonly IPreGameUiController _preGameUiController;

		private ICharacterLiveController<AiHitInfo> _aiLiveController;
		private AiTreeContext _treeContext;

		public PositionPoint CurrentPositionPoint { get; private set; }
		public bool HasPositionPoint => CurrentPositionPoint != null;

		public AiBehaviourController(ICharacterEntity entity,
			IBehaviourTreeManager behaviourTreeManager,
			ICharacterSpawnContext spawnContext,
			IBattleService<BattleContext> battleService,
			IInstantiator instantiator,
			IPreGameUiController preGameUiController)
		{
			_entity = entity;
			_behaviourTreeManager = behaviourTreeManager;
			_spawnContext = spawnContext;
			_battleService = battleService;
			_instantiator = instantiator;
			_preGameUiController = preGameUiController;
		}

		public override void OnContextBegin()
		{
			_entity.CurrentExecutionContext.TryResolveController(out _aiLiveController);

			_treeContext ??= _instantiator.Instantiate<AiTreeContext>();
			_treeContext.Construct(_entity.Transform);
			_treeContext.InitializeContext();
		}

		public override void Initialize()
		{
			_treeContext.IsCycled = true;
			_spawnContext.BehaviourTree.Bind(_treeContext);
			_behaviourTreeManager.Add(_spawnContext.BehaviourTree);

			_aiLiveController.OnEntityDead += DeadHandler;
			_aiLiveController.OnEntityAlive += AliveHandler;
			_battleService.OnAlarmRaised += OnAlarmRaised;
			_battleService.OnFirstShootRaised += OnFirstShootRaised;
			_battleService.OnAllEnemiesDeauthorizedInCurrentGroup += OnAllEnemiesDeadInCurrentGroup;
			_preGameUiController.OnTapToPlayClicked += OnTapToPlayClicked;
		}


		public override void Deinitialize()
		{
			_aiLiveController.OnEntityDead -= DeadHandler;
			_aiLiveController.OnEntityAlive -= AliveHandler;
			_battleService.OnAlarmRaised -= OnAlarmRaised;
			_battleService.OnFirstShootRaised -= OnFirstShootRaised;
			_battleService.OnAllEnemiesDeauthorizedInCurrentGroup -= OnAllEnemiesDeadInCurrentGroup;
			_preGameUiController.OnTapToPlayClicked -= OnTapToPlayClicked;
			
			_spawnContext.BehaviourTree.Dispose();
		}

		public bool TryGetPositionPoint(out PositionPoint positionPoint)
		{
			positionPoint = CurrentPositionPoint;
			
			return HasPositionPoint;
		}


		public virtual void SetPositionPoint(PositionPoint positionPoint)
		{
			bool isDestractablePosition = positionPoint.IsCoverPoint && positionPoint.IsDestractable;
			if (isDestractablePosition)
			{
				if (HasPositionPoint)
					CurrentPositionPoint.OnCoverDestroy -= OnCoverDestroy;

				_spawnContext.BehaviourTree.Blackboard.SetValue(BlackboardConst.IS_COVER_DESTROYED, false);

				positionPoint.OnCoverDestroy += OnCoverDestroy;

				if (positionPoint.IsDestroyed)
					_spawnContext.BehaviourTree.Blackboard.SetValue(BlackboardConst.IS_COVER_DESTROYED, true);
			}

			CurrentPositionPoint = positionPoint;
		}

		private void OnAlarmRaised()
		{
			_spawnContext.BehaviourTree.Blackboard.SetValue(BlackboardConst.WAS_FIRST_PLAYER_COVER_OUT, true);
		}


		private void OnFirstShootRaised()
		{
			_spawnContext.BehaviourTree.Blackboard.SetValue(BlackboardConst.WAS_FIRST_LOUD_SHOOT, true);
		}


		private void OnCoverDestroy()
		{
			_spawnContext.BehaviourTree.Blackboard.SetValue(BlackboardConst.IS_COVER_DESTROYED, true);
		}

		private void OnAllEnemiesDeadInCurrentGroup(int groupId)
		{
			_spawnContext.BehaviourTree.Blackboard.SetValue(BlackboardConst.WAS_FIRST_PLAYER_COVER_OUT, false);
			_spawnContext.BehaviourTree.Blackboard.SetValue(BlackboardConst.WAS_FIRST_LOUD_SHOOT, false);
		}

		private void OnTapToPlayClicked()
		{
			_spawnContext.BehaviourTree.OnTapToPlayClicked();
		}


		private void DeadHandler(ICharacterEntity entity)
		{
			_behaviourTreeManager.Remove(_spawnContext.BehaviourTree);
		}

		private void AliveHandler(ICharacterEntity entity)
		{
			// Ensure BT restarts cleanly from the beginning
			_spawnContext.BehaviourTree.RootNode.State = NodeState.Disabled;
			_behaviourTreeManager.Add(_spawnContext.BehaviourTree);
			_spawnContext.BehaviourTree.Run();
		}
	}
}
