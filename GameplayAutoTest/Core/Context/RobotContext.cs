using System.Collections.Generic;
using Swat.Game.GameplayRobot.Core.Interfaces;
using Swat.Game.GameplayRobot.Core.Logger;
using Swat.Game.GameplayRobot.Core.Logger.Data;
using Swat.Game.GameplayRobot.Core.Task;

namespace Swat.Game.GameplayRobot.Core.Context
{
	public abstract class RobotContext : IRobotContext
	{
		protected IGameServiceAdapter _game;
		
		private readonly List<IRobotTask> _tasksPriority = new();
		private Queue<IRobotTask> _tasksQueue = new();
		private IRobotTask _currentTask;

		public abstract bool IsLooped { get; }

		public virtual void Initialize(IGameServiceAdapter service)
		{
			_game = service;
			
			RegisterTasks();
			
			_tasksQueue = new Queue<IRobotTask>(_tasksPriority);
		}

		public virtual void OnEnter()
		{
			RobotLogger.Log(LogLevel.Info, $"Context entered: {GetType().Name}");

			ResetQueue();
			TrySetNextTask();
		}

		public virtual void OnExit()
		{
			_currentTask?.Reset();
			_currentTask = null;

			foreach (var task in _tasksPriority)
				task.Reset();

			RobotLogger.Log(LogLevel.Debug, $"Context exited: {GetType().Name}");
		}

		public virtual void ExecuteLogic()
		{
			if(_currentTask == null)
				return;

			if (!_currentTask.CanExecute() || _currentTask.IsCompleted)
			{
				TrySetNextTask();
				return;
			}

			_currentTask.Execute();
		}

		public void Reset()
		{
			_currentTask = null;
			ResetQueue();
		}
		
		protected abstract void RegisterTasks();
		
		protected void AddTask(IRobotTask task)
		{
			_tasksPriority.Add(task);
		}
		
		private void TrySetNextTask()
		{
			if (_tasksQueue.Count == 0)
			{
				if(IsLooped)
				{
					ResetQueue();
				}
				else
				{
					_currentTask = null;
					return;
				}
			}

			_currentTask?.Reset();
			_currentTask = _tasksQueue.Dequeue();
			_currentTask.OnStart();
			
			RobotLogger.Log(LogLevel.Debug, $"Task switched to: {_currentTask?.GetType().Name}");
		}

		private void ResetQueue()
		{
			_tasksQueue.Clear();
			
			foreach (var task in _tasksPriority)
				_tasksQueue.Enqueue(task);
		}
	}
}