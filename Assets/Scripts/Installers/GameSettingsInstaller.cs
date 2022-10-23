using System;
using Ball;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Installers
{
	[CreateAssetMenu(fileName = "GameSettingsInstaller", menuName = "Installers/GameSettingsInstaller")]
	public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
	{
		[SerializeField] private GameSettings gameSettings;

		public override void InstallBindings()
		{
			Container.Bind<GameSettings>().FromInstance(gameSettings).AsSingle();
		}
	}

	[Serializable]
	public class GameSettings
	{
		[SerializeField] private int            countDownSeconds = 3;
		[SerializeField] private byte           maxScore         = 10;
		[SerializeField] private NetworkObject  playerPrefab;
		[SerializeField] private GameObject     playerViewPrefab;
		[SerializeField] private BallController ballPrefab;
		[SerializeField] private float          startBallSpeed;
		[SerializeField] private float          startBallRadius;
		[SerializeField] private float          startPlayerHeight;
		[SerializeField] private float          startPlayerWidth;
		[SerializeField] private float          increaseBallSpeedByTick = 0.0001f;
		[SerializeField] private float          cameraSize              = 5;
		[SerializeField] private float          playerSpeed             = 5;
		[SerializeField] private AnimationCurve playerSpeedCurve;
		[SerializeField] private float          bonusTimer = 20;

		public int            CountDownSeconds        => countDownSeconds;
		public byte           MaxScore                => maxScore;
		public NetworkObject  PlayerPrefab            => playerPrefab;
		public GameObject     PlayerViewPrefab        => playerViewPrefab;
		public BallController BallPrefab              => ballPrefab;
		public float          StartBallSpeed          => startBallSpeed;
		public float          StartBallRadius         => startBallRadius;
		public float          StartPlayerHeight       => startPlayerHeight;
		public float          StartPlayerWidth        => startPlayerWidth;
		public float          IncreaseBallSpeedByTick => increaseBallSpeedByTick;
		public float          CameraSize              => cameraSize;
		public float          PlayerSpeed             => playerSpeed;
		public AnimationCurve PlayerSpeedCurve        => playerSpeedCurve;
		public float          BonusTimer              => bonusTimer;
	}
}