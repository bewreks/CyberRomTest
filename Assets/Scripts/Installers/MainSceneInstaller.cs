using System;
using Physics;
using Player;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace Installers
{
	public class MainSceneInstaller : MonoInstaller
	{
		[SerializeField] private TextMeshProUGUI countdownText;
		[SerializeField] private TextMeshProUGUI topPlayerScoreText;
		[SerializeField] private TextMeshProUGUI bottomPlayerScoreText;

		[Inject] private GameSettings      _gameSettings;
		[Inject] private PhysicsController _physicsController;
		[Inject] private GameResult        _gameResult;

		private int _playersInLobby;

		private CompositeDisposable _disposables = new();
		private IDisposable         _timer;
		private int                 _countdowns;

		public override void InstallBindings()
		{
			Container                      =  ProjectContext.Instance.Container;
			Events.Events.OnCountDownStart += OnCountDownStart;
			Events.Events.UpdateScore      += OnUpdateScore;
			Events.Events.GameEndClient    += OnGameEndClient;

			_physicsController.AddTo(_disposables);

			countdownText.text = "";
			countdownText.gameObject.SetActive(false);
		}

		private void OnGameEndClient(PlayerType loser, PlayerType winner)
		{
			_gameResult.Loser  = loser;
			_gameResult.Winner = winner;

			if (winner == PlayerType.Bottom)
			{
				(_gameResult.WinnerScore, _gameResult.LoserScore) = (_gameResult.LoserScore, _gameResult.WinnerScore);
			}
		}

		private void OnUpdateScore(byte bottomScore, byte topScore)
		{
			var topScoreCalc    = _gameSettings.MaxScore - bottomScore;
			var bottomScoreCals = _gameSettings.MaxScore - topScore;

			_gameResult.WinnerScore = topScore;
			_gameResult.LoserScore  = bottomScore;

			topPlayerScoreText.text    = topScoreCalc.ToString();
			bottomPlayerScoreText.text = bottomScoreCals.ToString();
		}

		[Inject]
		public void Construct()
		{
			_countdowns = _gameSettings.CountDownSeconds;
		}

		private void OnCountDownStart()
		{
			countdownText.gameObject.SetActive(true);
			countdownText.text = $"{_countdowns}";
			_timer             = Observable.Timer(TimeSpan.FromSeconds(1)).Repeat().Subscribe(OnTimerTick);
			_timer.AddTo(_disposables);
		}

		private void OnTimerTick(long obj)
		{
			countdownText.text = $"{--_countdowns}";

			if (_countdowns <= 0)
			{
				countdownText.text = "";
				countdownText.gameObject.SetActive(false);
				_timer.Dispose();
				_disposables.Remove(_timer);
				_timer = null;
				Events.Events.OnCountDownEndInvoke();
			}
		}

		private void OnDestroy()
		{
			_disposables?.Dispose();

			Events.Events.OnCountDownStart -= OnCountDownStart;
			Events.Events.UpdateScore      -= OnUpdateScore;

			Container.Unbind<PhysicsController>();
		}
	}
}