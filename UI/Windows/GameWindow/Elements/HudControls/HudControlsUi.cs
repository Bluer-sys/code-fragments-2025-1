using Swat.Game.Services.WindowsService;
using Swat.Game.UI.Windows.ControlsSettings.Data;
using Swat.Utils;
using UnityEngine;

namespace Swat.Game.UI.Windows.GameWindow.Elements.HudControls
{
	public class HudControlsUi : BaseWindow
	{
		[field: SerializeField] public SerializableDictionary<ControlElement, RectTransform> Controls { get; private set; }

		[SerializeField] private GameObject twoFingersControlsObject;
		[SerializeField] private GameObject defaultControlObject;

		public void ActivateControls(bool isAlternative)
		{
			twoFingersControlsObject.SetActive(isAlternative);
			defaultControlObject.SetActive(true);
		}

		public void SetCoverButtonActive(bool isActive)
		{
			Controls[ControlElement.CoverButton].gameObject.SetActive(isActive);
		}
	}
}