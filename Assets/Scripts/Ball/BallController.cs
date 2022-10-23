using System;
using Bonuses;
using Installers;
using Physics;
using Player;
using UniRx;
using Unity.Netcode;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Ball
{
	public class BallController : NetworkBehaviour
	{
		[Inject] private PhysicsController _physicsController;
		[Inject] private GameSettings      _gameSettings;

		private BonusFilter _speedBonusFilter = BonusFilterFactory.GetNoneFilter();
		private BonusFilter _sizeBonusFilter  = BonusFilterFactory.GetNoneFilter();

		private IDisposable _previousSizeBonus;
		private IDisposable _previousSpeedBonus;

		public float Speed
		{
			get => _speedBonusFilter.Value(_speed);
			private set => _speed = value;
		}

		public float Size
		{
			get => _sizeBonusFilter.Value(_size);
			private set => _size = value;
		}

		public Vector3 Direction { get; private set; }

		public PlayerType CurrentPlayer
		{
			get => currentPlayer;
			private set
			{
				currentPlayer = value;

				Events.Events.CurrentPlayerInvoke(CurrentPlayer);
			}
		}


		private CompositeDisposable _disposables = new();

		private float      _speed;
		private float      _size;
		private PlayerType currentPlayer;


		public override void OnNetworkSpawn()
		{
			if (IsServer)
			{
				Pause();

				Events.Events.RestartBall   += OnRestartBall;
				Events.Events.GameEndServer += OnGameEndServer;
				Events.Events.ApplyBonus    += OnApplyBonus;
				ObservableHelper.EveryFixedUpdate.Subscribe(OnTick).AddTo(_disposables);

				_physicsController.RegisterBall(this);
			}
		}

		private void OnApplyBonus(BonusModel bonus)
		{
			if (bonus.relationship == BonusRelationship.Ball)
			{
				switch (bonus.type)
				{
					case BonusTypes.Size:
						BonusFilterFactory.Release(_sizeBonusFilter);
						_sizeBonusFilter = BonusFilterFactory.GetFilter(bonus);
						_previousSizeBonus?.Dispose();
						_previousSizeBonus = bonus.StartTimer(() =>
						{
							BonusFilterFactory.Release(_sizeBonusFilter);
							_sizeBonusFilter = BonusFilterFactory.GetNoneFilter();
							_previousSizeBonus?.Dispose();
							_previousSizeBonus = null;
						});
						break;
					case BonusTypes.Speed:
						BonusFilterFactory.Release(_speedBonusFilter);
						_speedBonusFilter = BonusFilterFactory.GetFilter(bonus);
						_previousSpeedBonus = bonus.StartTimer(() =>
						{
							BonusFilterFactory.Release(_speedBonusFilter);
							_speedBonusFilter = BonusFilterFactory.GetNoneFilter();
							_previousSpeedBonus?.Dispose();
							_previousSpeedBonus = null;
						});
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		private void OnGameEndServer()
		{
			Destroy(gameObject);
		}

		private void OnTick(Unit u)
		{
			_speed += _gameSettings.IncreaseBallSpeedByTick;
		}

		public override void OnDestroy()
		{
			if (IsServer)
			{
				Events.Events.RestartBall   -= OnRestartBall;
				Events.Events.GameEndServer -= OnGameEndServer;
				Events.Events.ApplyBonus    -= OnApplyBonus;
			}
		}

		private void OnRestartBall()
		{
			Restart();
		}

		public void Pause()
		{
			Speed = 0;
		}

		public void Restart()
		{
			Speed              = _gameSettings.StartBallSpeed;
			Size               = _gameSettings.StartBallRadius;
			transform.position = Vector3.zero;


			do
			{
				Direction = Random.insideUnitCircle;
			} while (Mathf.Abs(Direction.x) <= 0.3f ||
			         Mathf.Abs(Direction.y) <= 0.3f);

			CurrentPlayer = Direction.y < 0 ? PlayerType.Top : PlayerType.Bottom;
		}

		public void FlipX()
		{
			var direction = Direction;
			direction.x *= -1;
			Direction   =  direction;
		}

		public void FlipY()
		{
			var direction = Direction;
			direction.y *= -1;
			Direction   =  direction;

			CurrentPlayer = CurrentPlayer.OppositeSide();
		}
	}
}