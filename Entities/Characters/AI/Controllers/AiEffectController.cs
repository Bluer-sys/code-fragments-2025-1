using System.Collections.Generic;
using Swat.Game.Entities.Characters.AI.Effects;
using Swat.Game.Entities.Core.Characters.AI.Views;
using Swat.Game.Entities.Core.Characters.Common.Controllers;
using Swat.Game.Entities.Core.Characters.Effects;
using Swat.Game.Services.VfxService;
using UnityEngine;

namespace Swat.Game.Entities.Characters.AI.Controllers
{
	public class AiEffectController : BaseController, IEffectsController
	{
		private readonly ICharacterEntity _entity;
		private readonly IAiEffectsView _view;

		private bool _wasInitialized;
		private Dictionary<EffectType, IEffect> _activeEffects;

		public AiEffectController(ICharacterEntity entity, IAiEffectsView view)
		{
			_entity = entity;
			_view = view;
		}


		public override void Initialize()
		{
			_activeEffects = new Dictionary<EffectType, IEffect>();

			_wasInitialized = true;
		}


		public override void Deinitialize()
		{
			if (!_wasInitialized)
				return;

			foreach (var effect in _activeEffects.Values)
			{
				effect.Deinitialize();
			}

			_activeEffects.Clear();
		}


		public void PlayVfx(VfxType effectType)
		{
			ParticleSystem particleSystem = _view.Vfxs[effectType];

			particleSystem.gameObject.SetActive(true);
			particleSystem.Play();
		}

		public bool TryAddEffect(EffectType effectType)
		{
			if (effectType == EffectType.None)
			{
				Debug.LogError("Attempt to apply None effect");
				return false;
			}

			if (_activeEffects.ContainsKey(effectType))
			{
				Debug.LogError($"Effect {effectType} already applied");
				return false;
			}

			switch (effectType)
			{
				case EffectType.Bleeding:
					BleedingEffect bleedingEffect = new(_entity);
					bleedingEffect.Initialize();
					_activeEffects.Add(effectType, bleedingEffect);
					break;
			}

			return true;
		}


		public bool RemoveEffect(EffectType effectType)
		{
			if (effectType == EffectType.None)
			{
				Debug.LogError("Attempt to remove None effect");
				return false;
			}

			if (!_activeEffects.ContainsKey(effectType))
			{
				Debug.LogError($"Effect {effectType} does not contains at active effects");
				return false;
			}

			IEffect effect = _activeEffects[effectType];
			effect.Deinitialize();
			_activeEffects.Remove(effectType);

			return true;
		}
	}
}