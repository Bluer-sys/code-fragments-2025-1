using System;
using Cysharp.Threading.Tasks;
using DevMenu;
using DG.Tweening;
using Swat.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Swat.Game.Services.WindowsService.Windows
{
	public class GameWindow : BaseWindow
	{
		public event Action OnPauseClicked;
		
		[field: SerializeField] public CanvasGroup AnimationCanvasGroup { get; private set; }
		[field: SerializeField] public CanvasGroup VisibilityCanvasGroup { get; private set; }
		[field: SerializeField] public GraphicRaycaster GraphicRaycaster { get; private set; }
		
		[SerializeField] private Button _pauseButton;
		[SerializeField] private float animationDuration;
		[SerializeField] private float animationDelay;
		[SerializeField] private SafeArea[] safeAreas;

        private void Start()
        {
            _pauseButton.onClick.AddListener(OnPauseClickedHandler);
        }
        
		public async UniTask PlayHideAnimation()
		{
			AnimationCanvasGroup.alpha = 1.0f;
			await PlayFadeAsync(0.0f, 0.0f);
		}


		public async UniTask PlayShowAnimation()
		{
			AnimationCanvasGroup.alpha = 0.0f;
			await PlayFadeAsync(1.0f, animationDelay);
		}


		public void RefreshSafeArea()
		{
			if (safeAreas == null)
				return;

			foreach (var safeArea in safeAreas) safeArea.Refresh(true);
		}


		private void OnPauseClickedHandler()
		{
			OnPauseClicked?.Invoke();
		}


		private async UniTask PlayFadeAsync(float targetAlpha, float delay)
		{
			AnimationCanvasGroup.DOFade(targetAlpha, animationDuration).SetDelay(delay);
			await UniTask.WaitWhile(() => AnimationCanvasGroup.alpha.IsApproximately(targetAlpha), cancellationToken: AnimationCanvasGroup.gameObject.GetCancellationTokenOnDestroy());
		}
	}
}
