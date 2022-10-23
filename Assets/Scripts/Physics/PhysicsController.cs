using System;
using System.Collections.Generic;
using Ball;
using CameraTools;
using Installers;
using Player;
using UniRx;
using UnityEngine;
using Zenject;

namespace Physics
{
	public class PhysicsController : IDisposable
	{
		[Inject] private Camera       _camera;
		[Inject] private GameSettings _gameSettings;
		[Inject] private DiContainer  _container;

		private CompositeDisposable _disposables = new();

		private BallController _ball;

		private Rect _gameScreenCollider;

		private Dictionary<PlayerType, PlayerController> _players = new();

		public PhysicsController()
		{
			Events.Events.StartControllers   += OnStartControllers;
			Events.Events.RegisterPlayer += OnRegisterPlayer;
			Events.Events.GameEndServer += OnGameEndServer;
		}

		private void OnGameEndServer()
		{
			Dispose();
		}

		private void OnRegisterPlayer(PlayerType type, PlayerController controller)
		{
			_players.Add(type, controller);
		}

		private void OnStartControllers()
		{
			Start();
		}

		public void Start()
		{
			_gameScreenCollider = CameraHelper.WorldCameraRect(_gameSettings.CameraSize);
			_disposables        = new CompositeDisposable();
			ObservableHelper.EveryLateFixedUpdate.Subscribe(OnFixedUpdate).AddTo(_disposables);
		}

		private void OnFixedUpdate(Unit u)
		{
			if (_ball != null)
			{
				var oldPosition = _ball.transform.position;
				var newPosition = oldPosition + _ball.Direction * (Time.fixedDeltaTime * _ball.Speed);

				var notReady = true;

				while (notReady)
				{
					notReady = false;

					var rightWall = _gameScreenCollider.xMin + _ball.Size;

					if (Math.Abs(Mathf.Sign(newPosition.y) - Mathf.Sign(oldPosition.y)) > Mathf.Epsilon)
					{
						Events.Events.BallIntersectsCenterInvoke();
					}

					if (newPosition.x <= rightWall)
					{
						_ball.FlipX();
						newPosition.x     =  rightWall - (newPosition.x - rightWall);
						notReady          =  true;
					}

					var leftWall = _gameScreenCollider.xMax - _ball.Size;

					if (newPosition.x >= leftWall)
					{
						_ball.FlipX();
						newPosition.x     =  leftWall - (newPosition.x - leftWall);
						notReady          =  true;
					}

					var bottomWall = _gameScreenCollider.yMin + _gameSettings.StartPlayerHeight;

					if (newPosition.y <= bottomWall)
					{
						if (!CheckPlayerCollide(PlayerType.Bottom,
						                        oldPosition,
						                        newPosition))
						{
							Events.Events.LooseRoundInvoke(PlayerType.Bottom);
							return;
						}

						_ball.FlipY();
						newPosition.y     =  bottomWall - (newPosition.y - bottomWall);
						notReady          =  true;
					}

					var topWall = _gameScreenCollider.yMax - _gameSettings.StartPlayerHeight;

					if (newPosition.y >= topWall)
					{
						if (!CheckPlayerCollide(PlayerType.Top,
						                        oldPosition,
						                        newPosition))
						{
							Events.Events.LooseRoundInvoke(PlayerType.Top);
							return;
						}

						_ball.FlipY();
						newPosition.y     =  topWall - (newPosition.y - topWall);
						notReady          =  true;
					}

					oldPosition = newPosition;
				}

				_ball.transform.position = newPosition;
			}
		}

		private bool CheckPlayerCollide(PlayerType type, Vector3 start, Vector3 end)
		{
			if (_players.TryGetValue(type, out var controller))
			{
				
				var playerPosition = controller.transform.position;
				Debug.DrawLine(start, end, Color.magenta, 1);
				Debug.DrawLine(controller.serverPlayerModel.Left + playerPosition, 
				               controller.serverPlayerModel.Right + playerPosition, 
				               Color.green, 1);
				return MathHelper.IsLinesIntersects(start, 
				                                    end, 
				                                    controller.serverPlayerModel.Left + playerPosition,
				                                    controller.serverPlayerModel.Right + playerPosition, 
				                                    out var intersected);
			}

			return true;
		}

		public void Stop()
		{
			_disposables?.Dispose();
		}

		public void Dispose()
		{
			Events.Events.StartControllers   -= OnStartControllers;
			Events.Events.RegisterPlayer -= OnRegisterPlayer;
			Events.Events.GameEndServer  -= OnGameEndServer;
			_players.Clear();
			_disposables?.Dispose();
		}

		public void RegisterBall(BallController ball)
		{
			_ball = ball;
		}
	}
}