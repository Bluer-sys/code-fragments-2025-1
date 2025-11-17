using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using Swat.Data;
using Swat.Data.Game;
using Swat.Game.Core;
using Swat.Game.Data.Level.Segment;
using Swat.Game.Entities.Characters.Player.Models;
using Swat.Game.Entities.Core.Characters.Common.Controllers;
using Swat.Game.Entities.Core.Characters.Player.Controllers;
using Swat.Game.Entities.Core.Characters.Player.Views;
using Swat.Game.GameControllers;
using Swat.Game.Services.BattleService;
using Swat.Game.Services.LevelService.Data;
using Swat.Game.Services.SceneRoutesProvider.Core;
using Swat.Utils;
using UnityEngine;

namespace Swat.Game.Entities.Characters.Player.Controllers
{
	public class PlayerCoverTransitionController : BaseController, IPlayerCoverTransitionController, IUpdatable
	{
		public event Action OnCoverReached;
		public event Action OnCoverLeave;
		public event Action OnCoverDestructed;
		public event Action<float> OnToCoverPercentChanged;
		private readonly PlayerCoverTransitionModel model;
		private readonly IBattleService<BattleContext> battleService;
		private readonly ICharacterEntity entity;
		private readonly IPlayerMovementView playerMovementView;
		private readonly SceneEvents sceneEvents;
		private readonly ISceneRoutesProvider sceneRoutesProvider;

		private IHitTargetController playerHitTargetController;
		private IPlayerAnalyticsController playerAnalyticsController;

		private IReadOnlyList<SegmentTransitionCondition> currentTransitionConditions;
		private bool isTransitionProcessActive;
		private int _currentCoverIndex;

		protected PlayerCoverTransitionController(PlayerCoverTransitionModel model,
			IBattleService<BattleContext> battleService,
			ICharacterEntity entity,
			IPlayerMovementView playerMovementView,
			SceneEvents sceneEvents,
			ISceneRoutesProvider sceneRoutesProvider)
		{
			this.model = model;
			this.battleService = battleService;
			this.entity = entity;
			this.playerMovementView = playerMovementView;
			this.sceneEvents = sceneEvents;
			this.sceneRoutesProvider = sceneRoutesProvider;
		}


		public override void OnContextBegin()
		{
			entity.CurrentExecutionContext.TryResolveController(out playerHitTargetController);
			entity.CurrentExecutionContext.TryResolveController(out playerAnalyticsController);

			sceneEvents.OnPlayerCoverReachedEvent += OnCoverReachedHandler;
			sceneEvents.OnHelicopterHoverAction += OnCoverHoverHandler;
		}


		public override void OnContextEnd()
		{
			sceneEvents.OnPlayerCoverReachedEvent -= OnCoverReachedHandler;
			sceneEvents.OnHelicopterHoverAction -= OnCoverHoverHandler;
		}


		public override void Deinitialize()
		{
			base.Deinitialize();

			OnContextEnd();
		}


		public void OnUpdate()
		{
			HandleCoverTransitionPossibility();
		}

		public PlayerCoverData GetNextCoverData()
		{
			var data = sceneRoutesProvider.SortedPlayerCoverData.ElementAtOrLast(_currentCoverIndex + 1);

			return data;
		}

		private void HandleCoverTransitionPossibility()
		{
			bool canTransit = IsTransitionPossible();

			if (canTransit)
				RealizeTransition();
		}

		private void OnCoverHoverHandler(AISpawnGroup aiSpawnGroup)
		{
			battleService.CurrentEnemyActiveGroupId = aiSpawnGroup.GetInstanceID();
		}


		private void OnCoverReachedHandler(PlayerCoverData reachedCover)
		{
			if (playerMovementView.Follower.spline != null 
				&& playerMovementView.Follower.spline != reachedCover.Spline)
				return;

			if(model.CurrentCoverData != null)
				model.CurrentCoverData.BreakableData.OnCoverDestructed -= OnCoverDestructedHandler;
			
			model.CurrentCoverData = reachedCover;
			model.IsCoverDestructed = false;

			_currentCoverIndex++;

			if (sceneRoutesProvider.PlayerCoverSegmentsMap.TryGetValue(reachedCover, out var segment))
			{
				int reachedSegmentId = segment.GetInstanceID();
				battleService.CurrentEnemyActiveGroupId = reachedSegmentId;
			}

#if UNITY_EDITOR
			LogAllEnemiesGroups(reachedCover);
#endif
			reachedCover.ActivateCover();
			reachedCover.BreakableData.OnCoverDestructed += OnCoverDestructedHandler;
			
			var transitionConditions = sceneRoutesProvider.GetSegmentTransitionConditions(reachedCover);
			RegisterCoverTransitionConditions(transitionConditions);

			isTransitionProcessActive = false;

			if (IsTransitionPossible())
			{
				RealizeTransition();
				return;
			}

			playerHitTargetController.SetupAdditionalHitPoints(reachedCover.UniqueHitPoints);
			OnCoverReached?.Invoke();
			playerAnalyticsController.OnCoverReached(_currentCoverIndex);
		}

		private void OnCoverDestructedHandler()
		{
			model.IsCoverDestructed = true;
			OnCoverDestructed?.Invoke();
		}


		private void RealizeTransition()
		{
			if (isTransitionProcessActive)
				return;

			isTransitionProcessActive = true;

			UnregisterCoverTransitionConditions();
			battleService.ReauthorizeAllEnemiesToNextGroup();

			OnCoverLeave?.Invoke();
			playerAnalyticsController.OnCoverLeft(_currentCoverIndex);
		}


		private bool IsTransitionPossible()
		{
			int currentSegmentId = model.CurrentCoverData != null 
								   && sceneRoutesProvider.PlayerCoverSegmentsMap.TryGetValue(model.CurrentCoverData, out var segment) 
				? segment.GetInstanceID() 
				: 0;

			bool canTransitByNoAliveEnemies = battleService.AliveEnemies.TryGetValue(currentSegmentId, out HashSet<ICharacterEntity> aliveEnemies) && aliveEnemies.Count == 0;
			bool canTransitByCondition = currentTransitionConditions is { Count: > 0 };

			if (canTransitByCondition)
			{
				foreach (SegmentTransitionCondition condition in currentTransitionConditions)
					canTransitByCondition &= condition.IsTrue;
			}

			bool canTransit = canTransitByNoAliveEnemies ||
							  canTransitByCondition;

			return canTransit;
		}


		private void RegisterCoverTransitionConditions(IReadOnlyList<SegmentTransitionCondition> data)
		{
			if (data == null)
				return;

			currentTransitionConditions = data;

			foreach (var condition in currentTransitionConditions)
			{
				condition.Register();
			}
		}


		private void UnregisterCoverTransitionConditions()
		{
			if (currentTransitionConditions == null)
				return;

			foreach (var condition in currentTransitionConditions)
			{
				condition.Unregister();
			}

			currentTransitionConditions = null;
		}


		private void LogAllEnemiesGroups(PlayerCoverData playerCoverData)
		{
			var sb = new StringBuilder();
			sb.Append($"Player Cover Reached {playerCoverData.HitPoints.FirstOrDefault()?.parent.name}\n");

			foreach ((int groupId, HashSet<ICharacterEntity> enemies) in battleService.AliveEnemies)
			{
				sb.Append("Group: " + groupId + "\n");

				foreach (ICharacterEntity enemy in enemies)
				{
					if (enemy.Transform.parent == null)
						sb.Append("\t" + enemy.Transform.name + "\n");
					else
						sb.Append("\t" + enemy.Transform.parent.name + "\n");
					
				}

				sb.Append("\n");
			}

			Debug.Log(sb.ToString());
		}
	}
}
