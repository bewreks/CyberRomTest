using System;
using Bonuses;
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
		[Inject] private GameSettings    _gameSettings;
		[Inject] private Camera          _sceneCamera;
		[Inject] private GameResult      _gameResult;
		[Inject] private BonusesSettings _bonusesSettings;

		private CompositeDisposable _disposables = new();

		private float _movingTime;

		private BonusFilter _speedFilter = BonusFilterFactory.GetNoneFilter();
		private BonusFilter _widthFilter = BonusFilterFactory.GetNoneFilter();
		private GameObject  _playerView;

		private float _viewWidth;


		private IDisposable _previousSizeBonus;
		private IDisposable _previousSpeedBonus;

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
			_playerView = Instantiate(_gameSettings.PlayerViewPrefab, transform);
			_viewWidth  = _playerView.transform.localScale.x;
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
			                      _speedFilter.Value(_gameSettings.PlayerSpeed);

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

		public override void OnNetworkDespawn()
		{
			_disposables?.Dispose();

			_previousSizeBonus?.Dispose();
			_previousSpeedBonus?.Dispose();

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

		[ClientRpc]
		public void ApplyBonusClientRpc(uint bonusId)
		{
			var bonus = _bonusesSettings.GetBonus(bonusId);

			switch (bonus.type)
			{
				case BonusTypes.Speed:
					BonusFilterFactory.Release(_speedFilter);
					_speedFilter = BonusFilterFactory.GetFilter(bonus);
					_previousSpeedBonus?.Dispose();
					_previousSpeedBonus = bonus.StartTimer(() =>
					{
						BonusFilterFactory.Release(_speedFilter);
						_speedFilter = BonusFilterFactory.GetNoneFilter();
						_previousSpeedBonus?.Dispose();
						_previousSpeedBonus = null;
					});
					break;
				case BonusTypes.Size:
					BonusFilterFactory.Release(_widthFilter);
					_widthFilter = BonusFilterFactory.GetFilter(bonus);
					var scale = _playerView.transform.localScale;
					scale.x                          = _widthFilter.Value(_viewWidth);
					_playerView.transform.localScale = scale;
					_previousSizeBonus?.Dispose();
					_previousSizeBonus = bonus.StartTimer(() =>
					{
						BonusFilterFactory.Release(_widthFilter);
						_widthFilter                     = BonusFilterFactory.GetNoneFilter();
						scale                            = _playerView.transform.localScale;
						scale.x                          = _widthFilter.Value(_viewWidth);
						_playerView.transform.localScale = scale;
						_previousSizeBonus?.Dispose();
						_previousSizeBonus = null;
					});
					break;
			}
		}

		public void ApplyBonus(BonusModel bonus)
		{
			serverPlayerModel.ApplyBonus(bonus);
			ApplyBonusClientRpc(bonus.id);
		}
	}
}