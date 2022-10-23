using System;
using Player;

namespace Events
{
	public static partial class Events
	{
		public static event Action                               OnCountDownStart;
		public static event Action                               OnCountDownEnd;
		public static event Action                               StartPhysics;
		public static event Action                               RestartBall;
		public static event Action<object>                       Inject;
		public static event Action<PlayerType>                   LooseRound;
		public static event Action<PlayerType, PlayerType>       GameEndClient;
		public static event Action                               GameEndServer;
		public static event Action<PlayerType, PlayerController> RegisterPlayer;
		public static event Action<byte, byte>                   UpdateScore;

		public static void OnCountDownStartInvoke()
		{
			OnCountDownStart?.Invoke();
		}

		public static void OnCountDownEndInvoke()
		{
			OnCountDownEnd?.Invoke();
		}

		public static void InjectInvoke(object obj)
		{
			Inject?.Invoke(obj);
		}

		public static void LooseRoundInvoke(PlayerType player)
		{
			LooseRound?.Invoke(player);
		}

		public static void StartPhysicsInvoke()
		{
			StartPhysics?.Invoke();
		}

		public static void UpdateScoreInvoke(byte bottomScore, byte topScore)
		{
			UpdateScore?.Invoke(bottomScore, topScore);
		}

		public static void BallRestartInvoke()
		{
			RestartBall?.Invoke();
		}

		public static void RegisterPlayerInvoke(PlayerType type, PlayerController controller)
		{
			RegisterPlayer?.Invoke(type, controller);
		}

		public static void GameEndClientInvoke(PlayerType loser, PlayerType winner)
		{
			GameEndClient?.Invoke(loser, winner);
		}

		public static void GameEndServerInvoke()
		{
			GameEndServer?.Invoke();
		}

		public static void ClearEvents()
		{
			OnCountDownStart = null;
			OnCountDownEnd   = null;
			StartPhysics     = null;
			RestartBall      = null;
			Inject           = null;
			LooseRound       = null;
			GameEndClient    = null;
			GameEndServer    = null;
			RegisterPlayer   = null;
			UpdateScore      = null;
		}
	}
}