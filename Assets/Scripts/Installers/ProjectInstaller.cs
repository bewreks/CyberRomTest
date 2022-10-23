using System;
using UniRx;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Installers
{
	public class ProjectInstaller : MonoInstaller
	{
		[Inject] private NetworkManager _networkManager;

		public override void InstallBindings()
		{
			Events.Events.OnHostButtonClick     += OnHostButtonClick;
			Events.Events.OnClientButtonClick   += OnClientButtonClick;
			Events.Events.OnShutdownButtonClick += OnShutdownButtonClick;
			DontDestroyOnLoad(new GameObject("Observable", typeof(MonoObservable)));
		}

		private void OnShutdownButtonClick()
		{
			_networkManager.Shutdown();
		}

		private void OnClientButtonClick(string ip)
		{
			((UnityTransport)_networkManager.NetworkConfig.NetworkTransport).ConnectionData.Address = ip;
			_networkManager.StartClient();
		}

		private void OnHostButtonClick()
		{
			_networkManager.StartHost();
		}

		private void OnDestroy()
		{
			Events.Events.OnHostButtonClick     -= OnHostButtonClick;
			Events.Events.OnClientButtonClick   -= OnClientButtonClick;
			Events.Events.OnShutdownButtonClick -= OnShutdownButtonClick;
		}

		public override void Start()
		{
			var args = Environment.GetCommandLineArgs();

			// Пока что задел на будущее с отдельным сервером
			foreach (var arg in args)
			{
				if (arg == "-server")
				{
					SceneManager.LoadScene("Scenes/ServerScene");
				}
			}
		}
	}


	public static class ObservableHelper
	{
		public static readonly ReactiveCommand EveryFixedUpdate     = new();
		public static readonly ReactiveCommand EveryLateFixedUpdate = new();

		public static void OnFixedUpdate()
		{
			EveryFixedUpdate.Execute();
		}

		public static void OnLateFixedUpdate()
		{
			EveryLateFixedUpdate.Execute();
		}

		public static void Dispose()
		{
			EveryFixedUpdate.Dispose();
			EveryLateFixedUpdate.Dispose();
		}
	}

	public class MonoObservable : MonoBehaviour
	{
		private void FixedUpdate()
		{
			ObservableHelper.OnFixedUpdate();
			ObservableHelper.OnLateFixedUpdate();
		}

		private void OnDestroy()
		{
			ObservableHelper.Dispose();
		}
	}
}