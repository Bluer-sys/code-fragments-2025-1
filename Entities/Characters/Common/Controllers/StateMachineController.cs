using System;
using System.Collections.Generic;
using Swat.Game.Entities.Core.Characters.Common.Controllers;
using Swat.Game.Entities.Core.Characters.States;
using UnityEngine;

namespace Swat.Game.Entities.Characters.Common.Controllers
{
	public abstract class StateMachineController : BaseController, IStateMachineBehaviour, IStateMachineController
	{
		public virtual IState LastState { get; protected set; }
		public virtual IState CurrentState { get; protected set; }

		protected virtual bool AllowSameStateSwitch => true;
		private readonly Dictionary<Type, IState> statesMap;


		protected StateMachineController(ICollection<IState> states)
		{
			statesMap = new Dictionary<Type, IState>();

			foreach (var state in states) statesMap.Add(state.GetType(), state);
		}


		public override void Initialize()
		{
			foreach (var state in statesMap)
			{
				state.Value.InitializeStateMachineBehaviour(this);
				state.Value.Initialize();
			}
		}


		public override void Deinitialize()
		{
			foreach (var state in statesMap) state.Value.Deinitialize();
		}


		public virtual void SwitchState(IState state)
		{
			LastState = CurrentState;

			if (!AllowSameStateSwitch
				&& CurrentState != null
				&& CurrentState.GetType() == state.GetType())
				return;

			CurrentState?.OnStateEnd();
			CurrentState = state;
			CurrentState.OnStateBegin();
		}


		public virtual void SwitchState<TState>() where TState : IState
		{
			if (!statesMap.TryGetValue(typeof(TState), out IState state))
			{
				Debug.LogError($"State with type <b>{typeof(TState)}</b> does not exists!");
				return;
			}

			SwitchState(state);
		}


		public virtual void SwitchState(Type stateType)
		{
			if (!statesMap.TryGetValue(stateType, out IState state))
			{
				Debug.LogError($"State with type <b>{stateType}</b> does not exists!");
				return;
			}

			SwitchState(state);
		}


		public void RewindState()
		{
		}

		public void Exit()
		{
		}


		public virtual void MoveToNextState()
		{
		}
	}
}