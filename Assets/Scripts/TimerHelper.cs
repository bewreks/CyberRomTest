using System;
using Bonuses;
using UniRx;

public static class TimerHelper
{
	public static IDisposable StartTimer(this BonusModel model, Action callback)
	{
		return Observable.Timer(TimeSpan.FromSeconds(model.time))
		                 .Subscribe(_ =>
		                 {
			                 callback?.Invoke();
		                 });
	}
}