using Installers;
using Physics;
using UniRx;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Ball
{
	public class BallController : NetworkBehaviour
	{
		[Inject] private PhysicsController _physicsController;
		[Inject] private GameSettings      _gameSettings;

		[HideInInspector] public float   speed = 2;
		[HideInInspector] public Vector3 direction;

		private CompositeDisposable _disposables = new();

		public override void OnNetworkSpawn()
		{
			if (IsServer)
			{
				Pause();

				Events.Events.RestartBall += OnRestartBall;
				Events.Events.GameEndServer += OnGameEndServer;
				ObservableHelper.EveryFixedUpdate.Subscribe(OnTick).AddTo(_disposables);

				_physicsController.RegisterBall(this);
			}
		}

		private void OnGameEndServer()
		{
			Destroy(gameObject);
		}

		private void OnTick(Unit u)
		{
			speed += _gameSettings.IncreaseBallSpeedByTick;
		}

		public override void OnDestroy()
		{
			if (IsServer)
			{
				Events.Events.RestartBall -= OnRestartBall;
			}
		}

		private void OnRestartBall()
		{
			Restart();
		}

		public void Pause()
		{
			speed = 0;
		}

		public void Restart()
		{
			speed              = _gameSettings.StartBallSpeed;
			transform.position = Vector3.zero;

			do
			{
				direction = Random.insideUnitCircle;
			} while (Mathf.Abs(direction.x) <= 0.3f ||
			         Mathf.Abs(direction.y) <= 0.3f);
		}
	}
}