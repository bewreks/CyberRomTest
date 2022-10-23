using System;
using Network;
using Player;
using TMPro;
using UniRx;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Installers
{
	public class EndSceneInstaller : MonoInstaller
	{
		[SerializeField] private TextMeshProUGUI result;
		[SerializeField] private Button          restart;
		
		[Inject] private Camera                _camera;
		[Inject] private GameResult            _gameResult;
		[Inject] private NetworkGameController _networkGameController;
		[Inject] private NetworkManager        _networkManager;

		public override void InstallBindings()
		{
			if (_gameResult != null)
			{
				if (_gameResult.OwnerType == _gameResult.Winner)
				{
					result.text = $"You win {_gameResult.WinnerScore} : {_gameResult.LoserScore}";
				}
				else
				{
					result.text = $"You lose {_gameResult.LoserScore} : {_gameResult.WinnerScore}";
				}
			}
			else
			{
				result.text = "";
				result.gameObject.SetActive(false);
			}
			Container = ProjectContext.Instance.Container;
		}

		private void OnDestroy()
		{
			restart.onClick.RemoveAllListeners();
			Container.Unbind<Camera>();
			Container.Unbind<GameResult>();
			Container.Unbind<NetworkGameController>();
			if (_camera)
				Destroy(_camera.gameObject);
		}

		public override void Start()
		{
			Events.Events.ClearEvents();
		
		
			Destroy(_networkGameController.gameObject);
			restart.onClick.AddListener(OnRestart);
			restart.gameObject.SetActive(false);
			IDisposable disposable = null;
			disposable = Observable.Timer(TimeSpan.FromSeconds(3f)).Subscribe(l =>
			{
				restart.gameObject.SetActive(true);
				// ReSharper disable once AccessToModifiedClosure
				disposable?.Dispose();
			});
		}

		private void OnRestart()
		{
			_networkManager.Shutdown();
			IDisposable disposable = null;
			disposable = Observable.Timer(TimeSpan.FromSeconds(.1f)).Subscribe(l =>
			{
				ScenesHelper.LoadScene(ScenesHelper.StartSceneName);
				// ReSharper disable once AccessToModifiedClosure
				disposable?.Dispose();
			});
			
		}
	}
}