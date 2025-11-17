using System;
using Cinemachine;
using DG.Tweening;
using Dreamteck.Splines;
using Swat.Game.Core;
using Swat.Game.Entities.Characters.Player.Views;
using Swat.Game.Entities.Core.Characters.Player.Controllers;
using UnityEngine;

namespace Swat.Game.Entities.Characters.Player.Controllers
{
	public class PlayerCutsceneCameraController : BaseController, IPlayerCutsceneCameraController
	{
		private const float CameraMoveSmoothSpeed = 15f;
		private readonly ICharacterEntity _entity;
		private readonly PlayerCutsceneCameraView _view;

		private ICameraController _cameraController;
		private IPlayerMovementController _playerMovementController;
		private CinemachineVirtualCamera _target;

		public PlayerCutsceneCameraController(ICharacterEntity entity, PlayerCutsceneCameraView view)
		{
			_entity = entity;
			_view = view;
		}


		public override void Initialize()
		{
			_entity.TryGetEntityController(out _cameraController);
			_entity.TryGetEntityController(out _playerMovementController);
		}


		public override void Deinitialize()
		{
		}

		public void ActiveCutsceneCamera(CinemachineVirtualCamera virtualCamera)
		{
			_target = virtualCamera;
			_cameraController.SetEnabled(false);
		}
		
		public void DeactivateCutsceneCamera(Action onComplete = null)
		{
			_target = null;
			
			MoveCamera(_view.Root.transform.position, _view.Root.transform.rotation);

			_cameraController.SmoothAlignCameraPoint(0.05f, onComplete: () =>
			{
				onComplete?.Invoke();
				_cameraController.SetEnabled(true);
			});
		}

		public void ManualUpdate()
		{
			if(_target == null)
				return;

			Vector3 targetPosition = _target.State.FinalPosition;
			Quaternion targetRotation = _target.State.FinalOrientation;
			
			// Debug.Log($"VCam Final Pos: {targetPosition} | VCam Final Rot: {targetRotation.eulerAngles}");
			
			MoveCameraSmooth(targetPosition, targetRotation);
		}

		private void MoveCamera(Vector3 position, Quaternion rotation)
		{
			_view.CameraRoot.position = position;
			_view.CameraRoot.rotation = rotation;
		}

		private void MoveCameraSmooth(Vector3 targetPosition, Quaternion targetRotation)
		{
			_view.CameraRoot.position = Vector3.Lerp(_view.CameraRoot.position, targetPosition, CameraMoveSmoothSpeed * Time.deltaTime);
			_view.CameraRoot.rotation = Quaternion.Lerp(_view.CameraRoot.rotation, targetRotation, CameraMoveSmoothSpeed * Time.deltaTime);
		}
	}
}