using System;
using Installers;
using Player;
using UniRx;
using Unity.Netcode;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Bonuses
{
	public class BonusController : IDisposable
	{
		[Inject] private BonusesSettings _bonusesSettings;
		[Inject] private GameSettings    _gameSettings;

		private IDisposable _bonusStartTimer;

		private PlayerType _currentPlayer;

		private BonusModel _currentBonus;
		private BonusView  _bonusView;

		public BonusController()
		{
			Events.Events.StartControllers += OnStartControllers;
		}

		private void OnCenterIntersection()
		{
			if (_currentBonus == null) return;

			_bonusView.Despawn();
			Events.Events.ApplyBonusInvoke(_currentBonus);
			_currentBonus = null;
			SpawnBonus();
		}

		private void OnStartControllers()
		{
			Start();
		}

		public void Start()
		{
			Events.Events.BallIntersectsCenter += OnCenterIntersection;
			SpawnBonus();
		}

		private void SpawnBonus()
		{
			_bonusStartTimer = Observable.Timer(TimeSpan.FromSeconds(_gameSettings.BonusTimer))
			                             .Subscribe(l =>
			                             {
				                             _currentBonus = _bonusesSettings.GetRandomBonus();
				                             _bonusView    = Object.Instantiate(_currentBonus.viewPrefab);
				                             _bonusView.Spawn();
			                             });
		}

		public void Dispose()
		{
			Events.Events.BallIntersectsCenter -= OnCenterIntersection;
			Events.Events.StartControllers     -= OnStartControllers;
			
			if (_bonusView)
				_bonusView.Despawn();
			_bonusStartTimer?.Dispose();
		}
	}
}