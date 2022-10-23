using Physics;
using Player;
using UnityEngine;
using Zenject;

namespace Installers
{
	public class LobbySceneInstaller : MonoInstaller
	{
		[SerializeField] private Camera gameCamera;

		[Inject] private GameSettings _gameSettings;

		public override void InstallBindings()
		{
			Container = ProjectContext.Instance.Container;

			gameCamera.orthographic     = true;
			gameCamera.orthographicSize = _gameSettings.CameraSize;

			DontDestroyOnLoad(gameCamera.gameObject);
			Container.Bind<GameResult>().FromNew().AsCached();
			Container.Bind<Camera>().FromInstance(gameCamera).AsCached();
			Container.Bind<PhysicsController>().FromNew().AsCached();
			Container.Inject(Container.Resolve<PhysicsController>());
		}
	}
}