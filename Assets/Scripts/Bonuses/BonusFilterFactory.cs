using System;
using System.Collections.Generic;

namespace Bonuses
{
	public static class BonusFilterFactory
	{
		private static Dictionary<Type, Queue<BonusFilter>> _filters = new();

		static BonusFilterFactory()
		{
			_filters.Add(typeof(AbsoluteFilter),        new Queue<BonusFilter>());
			_filters.Add(typeof(PercentDecreaseFilter), new Queue<BonusFilter>());
			_filters.Add(typeof(PercentIncreaseFilter), new Queue<BonusFilter>());
			_filters.Add(typeof(NoneFilter),            new Queue<BonusFilter>());
			_filters.Add(typeof(AdditionalFilter),      new Queue<BonusFilter>());
		}

		public static BonusFilter GetFilter(BonusModel model)
		{
			BonusFilter filter = GetNoneFilter();
			switch (model.valueType)
			{
				case BonusValueType.Absolute:
					filter = GetFilter<AbsoluteFilter>();
					break;
				case BonusValueType.PercentIncrease:
					filter = GetFilter<PercentIncreaseFilter>();
					break;
				case BonusValueType.PercentDecrease:
					filter = GetFilter<PercentDecreaseFilter>();
					break;
				case BonusValueType.Additional:
					filter = GetFilter<AdditionalFilter>();
					break;
			}

			filter.Initialize(model);
			return filter;
		}

		public static NoneFilter GetNoneFilter()
		{
			return GetFilter<NoneFilter>();
		}

		public static void Release<T>(T filter)
			where T : BonusFilter
		{
			_filters[filter.GetType()].Enqueue(filter);
		}

		private static T GetFilter<T>()
			where T : BonusFilter, new()
		{
			if (_filters[typeof(T)].Count == 0)
			{
				return new T();
			}
			return _filters[typeof(T)].Dequeue() as T;

		}
	}
}