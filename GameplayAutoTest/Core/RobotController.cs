using System;
using System.Collections.Generic;
using System.Linq;
using Swat.Game.GameplayRobot.Core.Context;
using Swat.Game.GameplayRobot.Core.Interfaces;
using Swat.Game.GameplayRobot.Core.Logger;
using Swat.Game.GameplayRobot.Core.Logger.Data;
using Swat.Game.GameplayRobot.Game.ArsenalContext;
using Swat.Game.GameplayRobot.Game.DefeatContext;
using Swat.Game.GameplayRobot.Game.GameContext;
using Swat.Game.GameplayRobot.Game.MainMenuContext;
using Swat.Game.GameplayRobot.Game.VictoryContext;
using Swat.Game.Services.WindowsService;
using Zenject;

namespace Swat.Game.GameplayRobot.Core
{
	public class RobotController : IRobotController, IInitializable, ITickable, IDisposable 
	{
		private const bool EnableLogging = true;

		private readonly IWindowsService _windowsService;
		private readonly IGameServiceAdapter _game;

		private Dictionary<WindowType, IRobotContext> _contextsPriority;
		private IRobotContext _currentContext;
		private bool _isActive;

		public RobotController(IWindowsService windowsService, IGameServiceAdapter game)
		{
			_windowsService = windowsService;
			_game = game;
		}
		
		public void Initialize()
		{
			RobotLogger.SetEnabled(false);

			_contextsPriority = new Dictionary<WindowType, IRobotContext>
			{
				{ WindowType.Victory, new VictoryRobotContext() },
				{ WindowType.ReviveWindow, new DefeatRobotContext() },
				{ WindowType.MapWindow, new MainMenuRobotContext() },
				{ WindowType.Arsenal, new ArsenalRobotContext() },
				{ WindowType.Game, new GameRobotContext() },
			};

			foreach (var kvp in _contextsPriority)
			{
				kvp.Value.Initialize(_game);
			}

			_windowsService.OnWindowActivated += OnWindowActivated;
			_windowsService.OnWindowDeactivated += OnWindowDeactivated;
			
			SetActivity(false);
		}

		public void Tick()
		{
			if (_isActive)
				_currentContext?.ExecuteLogic();
		}

		public void Dispose()
		{
			_windowsService.OnWindowActivated -= OnWindowActivated;
			_windowsService.OnWindowDeactivated -= OnWindowDeactivated;
		}
		
		public void SetActivity(bool isActive)
		{
			_isActive = isActive;

			// ReSharper disable once RedundantTernaryExpression
			RobotLogger.SetEnabled(isActive ? EnableLogging : false);
			
			if (isActive)
			{
				_currentContext?.OnEnter();
			}
			else 
			{
				_currentContext?.OnExit();

				foreach (var pair in _contextsPriority)
					pair.Value.Reset();
			}
			
			RobotLogger.Log(LogLevel.Info, $"Bot {(isActive ? "activated" : "deactivated")}");
		}
		
		private void OnWindowActivated(WindowType type)
		{
			_contextsPriority.TryGetValue(type, out var context);
			
			if (context == null)
			{
				RobotLogger.Log(LogLevel.Warning, $"No context for: {type}");
				return;
			}
			
			var targetContext = GetHighestPriorityContext();

			if (targetContext == _currentContext)
				return;
			
			if (_isActive)
				_currentContext?.OnExit();
				
			_currentContext = targetContext;

			if (_isActive)
				_currentContext.OnEnter();
		}

		private void OnWindowDeactivated(WindowType type)
		{
			_contextsPriority.TryGetValue(type, out var context);
			
			if (context == null)
				return;

			if (_currentContext != context)
				return;
			
			if (_isActive)
				_currentContext?.OnExit();
				
			_currentContext = GetHighestPriorityContext();

			if (_isActive)
				_currentContext?.OnEnter();
		}
		
		private IRobotContext GetHighestPriorityContext()
		{
			var windowsPriority = _contextsPriority.Keys.ToList();
			var targetWindow = WindowType.None;
			
			foreach (WindowType activeWindow in _windowsService.ActiveWindows)
			{
				if(!windowsPriority.Contains(activeWindow))
					continue;
				
				if (windowsPriority.IndexOf(activeWindow) < windowsPriority.IndexOf(targetWindow) 
					|| targetWindow == WindowType.None)
				{
					targetWindow = activeWindow;
				}
			}

			_contextsPriority.TryGetValue(targetWindow, out var context);
			
			return context;
		}
	}
}