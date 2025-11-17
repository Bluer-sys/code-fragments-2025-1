using UnityEngine;

namespace Swat.Game.UI.Windows.GameWindow.Core
{
	public interface ICrosshairUiController
	{
		Vector3 CrossHairWorldPosition { get; }

		Ray CrossHairRay { get; }


		void ApplyCrosshairRecoil(float value);

		void ShowCrossHair(bool isEnable);
		
		void UseCrossHairHitMarks(bool isUsing);

		void SetNoAmmoCrossEnabled(bool isEnable);
		
		void SetNoAmmoTextEnabled(bool isEnable);
	}
}