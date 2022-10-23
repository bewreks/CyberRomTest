using System;
using System.Collections.Generic;
using System.Linq;
using Ball;
using CameraTools;
using Installers;
using Physics;
using Player;
using UniRx;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Network
{
	public class NetworkGameController : NetworkBehaviour, IDisposable
	{
		[Inject] private GameSettings      _gameSettings;
		[Inject] private NetworkSettings   _networkSettings;
		[Inject] private DiContainer       _container;
		[Inject] private Camera            _camera;

		private Dictionary<ulong, PlayerModel>      _playersByID   = new();
		private Dictionary<PlayerType, PlayerModel> _playersByType = new();
		private GameState                           _state;

		private BallController _ball;

		public override void OnNetworkSpawn()
		{
			ProjectContext.Instance.Container.Inject(this);
			_container.Bind<NetworkGameController>().FromInstance(this);
			Events.Events.Inject += EventsOnInject;

			_state = GameState.Lobby;
			DontDestroyOnLoad(gameObject);

			if (IsServer)
			{
				NetworkManager.OnClientConnectedCallback   += OnConnect;
				NetworkManager.OnClientDisconnectCallback  += OnDisconnect;
				NetworkManager.SceneManager.OnLoadComplete += OnSceneLoadComplete;
				Events.Events.LooseRound                   += OnPlayerLoose;
			}

			if (IsHost)
			{
				OnConnect(OwnerClientId);
			}
		}

		private void OnPlayerLoose(PlayerType type)
		{
			if (_playersByType.ContainsKey(type))
			{
				_playersByType[type].DecreaseScore();
				
				var bottomScore = _gameSettings.MaxScore;
				var topScore    = _gameSettings.MaxScore;

				if (_playersByType.TryGetValue(PlayerType.Bottom, out var model))
				{
					bottomScore = model.Score;
				}
				if (_playersByType.TryGetValue(PlayerType.Top, out model))
				{
					topScore = model.Score;
				}
				UpdateScoreClientRpc(bottomScore, topScore);
				
				if (_playersByType[type].Score <= 0)
				{
					Events.Events.GameEndServerInvoke();
					GameEndClientRpc(type, type.OppositeSide());
					SwitchSceneToEnd();
					Dispose();
				}
				else
				{
					Events.Events.BallRestartInvoke();
				}
				
			}
		}

		private void EventsOnInject(object obj)
		{
			_container.Inject(obj);
		}

		private void OnSceneLoadComplete(ulong clientid, string scenename, LoadSceneMode loadscenemode)
		{
			if (_state != GameState.Game)
			{
				return;
			}

			var playerModel = _playersByID[clientid];
			playerModel.OnMainScene();
			var side = playerModel.Type.GetSide();
			var networkObject = Instantiate(_gameSettings.PlayerPrefab,
			                                new Vector3(0, side * _gameSettings.CameraSize),
			                                Quaternion.identity);

			networkObject.SpawnWithOwnership(clientid);
			playerModel.SetColliderSize(_gameSettings.StartPlayerWidth, _gameSettings.StartPlayerHeight);
			var playerController = networkObject.GetComponent<PlayerController>();
			playerController.SetModel(playerModel);
			playerController.SetSideClientRpc(playerModel.Type);
			Events.Events.RegisterPlayerInvoke(playerModel.Type,
			                                   playerController);
			CheckForAllLoaded();
		}

		private void CheckForAllLoaded()
		{
			if (_playersByID.All(pair => pair.Value.IsOnMainScene))
			{
				NetworkManager.SceneManager.OnLoadComplete -= OnSceneLoadComplete;
				Events.Events.StartPhysicsInvoke();
				_ball = Instantiate(_gameSettings.BallPrefab);
				_container.Inject(_ball);
				_ball.GetComponent<NetworkObject>().Spawn();
				IDisposable disposable = null;
				disposable = Observable.Timer(TimeSpan.FromSeconds(_gameSettings.CountDownSeconds)).Subscribe(l =>
				{
					_ball.Restart();
					// ReSharper disable once AccessToModifiedClosure
					disposable?.Dispose();
				});
				StartCountdownClientRpc();
			}
		}

		private void OnDisconnect(ulong clientId)
		{
			if (_playersByID.ContainsKey(clientId))
			{
				var playerModel = _playersByID[clientId];
				_playersByID.Remove(clientId);
				_playersByType.Remove(playerModel.Type);
			}
		}

		private void OnConnect(ulong clientId)
		{
			if (_state == GameState.Game)
			{
				NetworkManager.DisconnectClient(clientId);
				return;
			}

			if (!_playersByID.ContainsKey(clientId))
			{
				var playerModel = new PlayerModel(clientId, 
				                                  (PlayerType)(_playersByID.Count % 2),
				                                  _gameSettings.MaxScore);
				_playersByID.Add(clientId, playerModel);
				_playersByType.Add(playerModel.Type, playerModel);
			}

			if (_playersByID.Count == _networkSettings.PlayersCount)
			{
				NetworkManager.OnClientConnectedCallback  -= OnConnect;
				NetworkManager.OnClientDisconnectCallback -= OnDisconnect;
				_state                                    =  GameState.Game;
				SwitchSceneToMain();
			}
		}

		public override void OnDestroy()
		{
			if (IsServer)
			{
				NetworkManager.OnClientConnectedCallback  -= OnConnect;
				NetworkManager.OnClientDisconnectCallback -= OnDisconnect;
				if (NetworkManager.SceneManager != null)
					NetworkManager.SceneManager.OnLoadComplete -= OnSceneLoadComplete;
				Events.Events.LooseRound -= OnPlayerLoose;
			}
		}

		public void SwitchSceneToMain()
		{
			ScenesHelper.LoadScene(ScenesHelper.MainSceneName);
		}

		public void SwitchSceneToEnd()
		{
			ScenesHelper.LoadScene(ScenesHelper.EndSceneName);
		}

		[ClientRpc]
		private void StartCountdownClientRpc()
		{
			Events.Events.OnCountDownStartInvoke();
		}

		[ClientRpc]
		private void UpdateScoreClientRpc(byte bottomScore, byte topScore)
		{
			Events.Events.UpdateScoreInvoke(bottomScore, topScore);
		}

		[ClientRpc]
		private void GameEndClientRpc(PlayerType loser, PlayerType winner)
		{
			Events.Events.GameEndClientInvoke(loser, winner);
		}

		public void Dispose()
		{
			_playersByType.Clear();
			_playersByID.Clear();
			_state = GameState.Lobby;
			_ball  = null;
		}
	}

	public enum GameState
	{
		Lobby,
		Game
	}
}