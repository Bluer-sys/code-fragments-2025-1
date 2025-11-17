using System;
using Swat.Game.GameplayRobot.Core.Logger.Data;
using UnityEngine;

namespace Swat.Game.GameplayRobot.Core.Logger
{
	public static class RobotLogger
	{
		private static bool _isEnabled; 
		
		public static void SetEnabled(bool isEnabled)
		{
			_isEnabled = isEnabled;
		}

		public static void Log(LogLevel level, string message)
		{
			if(!_isEnabled)
				return;
			
			string formattedMessage = $"[Robot Ramil][{level}][{DateTime.Now:HH:mm:ss.fff}] {message}";

			switch (level)
			{
				case LogLevel.Critical:
				case LogLevel.Error:
					Debug.LogError(formattedMessage);
					break;
				case LogLevel.Warning:
					Debug.LogWarning(formattedMessage);
					break;
				default:
					Debug.Log(formattedMessage);
					break;
			}
		}
	}
}