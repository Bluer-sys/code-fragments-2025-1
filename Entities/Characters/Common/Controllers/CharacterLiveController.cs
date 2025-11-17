using System;
using DG.Tweening;
using Swat.Game.Entities.Characters.Common.Views;
using Swat.Game.Entities.Core;
using Swat.Game.Entities.Core.Characters.Common.Controllers;
using UnityEngine;

namespace Swat.Game.Entities.Characters.Common.Controllers
{
	public class CharacterLiveController<THitInfo> : BaseController, ICharacterLiveController<THitInfo>
		where THitInfo : struct
	{
		public event Action OnDead;
		public event Action<ICharacterEntity> OnEntityDead;
		public event Action<ICharacterEntity> OnEntityLeave;
		public event Action<ICharacterEntity> OnEntityAlive;
		public event Action OnHealthChanged;
		public event Action<ICharacterEntity, float> OnEntityHealthChangedPercent;

		public event Action<THitInfo> OnHit;


		protected CharacterLiveView LiveView { get; }
		private readonly ICharacterEntity _entity;

		protected bool _wasExploded;
		private Tween _immortalTween;

		protected CharacterLiveController(ICharacterEntity entity,
			CharacterLiveView liveView)
		{
			this._entity = entity;
			LiveView = liveView;
		}


		public override void Initialize()
		{
			SetAlive(true);
		}


		public override void Deinitialize()
		{
		}


		public void SetHealth(float value)
		{
			if (!LiveView.IsAlive || LiveView.IsImmortal)
				return;

			SetHealthForced(value);
		}

		public virtual void SetHealthForced(float value)
		{
			LiveView.Health = Mathf.Clamp(value, 0, float.MaxValue);

			float percent = LiveView.Health / LiveView.BaseHealth;
            
			OnHealthChanged?.Invoke();
			OnEntityHealthChangedPercent?.Invoke(_entity, percent);
            
			LiveView.HealthPercent = percent;
			OnHealthChangedHandler();

			if (LiveView.Health <= 0)
				SetAlive(false);
		}


		public void SetAlive(bool isAlive)
		{
			LiveView.IsAlive = isAlive;

			if (isAlive)
				OnReborn();
			else
				OnDeath();
		}


		public void SetImmortal(bool isImmortal)
		{
			LiveView.IsImmortal = isImmortal;
		}

		public void SetImmortal(float duration)
		{
			SetImmortal(true);

			_immortalTween?.Kill();
			_immortalTween = DOVirtual.DelayedCall(duration, () => SetImmortal(false))
				.OnComplete(() => _immortalTween = null);
		}

		public void Leave()
		{
			_entity.Transform.gameObject.SetActive(false);
			OnLeave();
			OnEntityLeave?.Invoke(_entity);
		}

		public virtual void ReceiveHit(THitInfo hitInfo)
		{
			InvokeOnHit(hitInfo);
		}

		protected void InvokeOnHit(THitInfo hitInfo)
		{
			OnHit?.Invoke(hitInfo);
		}


		public virtual void OnReborn()
		{
			OnEntityAlive?.Invoke(_entity);
		}

		public virtual void OnLeave()
		{
			SetAlive(false);
		}

		public virtual void OnDeath()
		{
			OnDead?.Invoke();
			OnEntityDead?.Invoke(_entity);
		}

		public virtual void SetDeathFromExplosion()
		{
			_wasExploded = true;
			SetAlive(false);
		}

		public void ResetHealth()
		{
			SetHealthForced(LiveView.BaseHealth);
		}

		public virtual void OnOtherHit(float damage)
		{
		}

		protected virtual void OnHealthChangedHandler()
		{
		}
	}
}
