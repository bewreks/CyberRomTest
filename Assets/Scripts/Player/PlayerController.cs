using CameraTools;
using Installers;
using UniRx;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Player
{
	public class PlayerController : NetworkBehaviour
	{
		[Inject] private GameSettings _gameSettings;
		[Inject] private Camera       _sceneCamera;
		[Inject] private GameResult   _gameResult;

		private CompositeDisposable _disposables = new();

		private float _movingTime;

		public PlayerModel serverPlayerModel { get; private set; }

		public override void OnNetworkSpawn()
		{
			Events.Events.InjectInvoke(this);

			if (IsOwner)
			{
				Events.Events.OnCountDownEnd += OnCountDownEnd;
			}

			if (IsServer)
			{
				Events.Events.GameEndServer += OnGameEndServer;
			}
		}

		private void OnGameEndServer()
		{
			var networkObject = GetComponent<NetworkObject>();
			networkObject.Despawn();
		}

		[Inject]
		private void Construct()
		{
			Instantiate(_gameSettings.PlayerViewPrefab, transform);
		}

		private void OnCountDownEnd()
		{
			ObservableHelper.EveryFixedUpdate.Subscribe(OnUpdate).AddTo(_disposables);
		}

		public void OnUpdate(Unit u)
		{
			_movingTime += Time.fixedDeltaTime;
			var worldMousePosition = CameraHelper.ScreenToWorldWithLockByX(_sceneCamera, _gameSettings.CameraSize);

			var fixedDeltaSpeed = _gameSettings.PlayerSpeedCurve.Evaluate(_movingTime) *
			                      Time.fixedDeltaTime *
			                      _gameSettings.PlayerSpeed;

			var startPosition = transform.position;
			worldMousePosition.y = startPosition.y;
			worldMousePosition.z = startPosition.z;
			worldMousePosition = Vector3.MoveTowards(startPosition,
			                                         new Vector3(worldMousePosition.x, startPosition.y),
			                                         fixedDeltaSpeed);

			if (transform.position == worldMousePosition)
			{
				_movingTime = 0;
			}
			transform.position = worldMousePosition;
		}

		public override void OnDestroy()
		{
			_disposables?.Dispose();

			if (IsOwner)
			{
				Events.Events.OnCountDownEnd -= OnCountDownEnd;
			}

			if (IsServer)
			{
				Events.Events.GameEndServer -= OnGameEndServer;
			}
		}

		[ClientRpc]
		public void SetSideClientRpc(PlayerType side)
		{
			_gameResult.OwnerType = side;
		}

		public void SetModel(PlayerModel playerModel)
		{
			serverPlayerModel = playerModel;
		}
	}
}