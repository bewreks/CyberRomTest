namespace Bonuses
{
	public abstract class BonusFilter
	{
		protected BonusModel _model;

		public void Initialize(BonusModel model)
		{
			_model = model;
		}

		public abstract float Value(float startValue);
	}

	public class NoneFilter : BonusFilter
	{
		public override float Value(float startValue)
		{
			return startValue;
		}
	}

	public class PercentIncreaseFilter : BonusFilter
	{
		public override float Value(float startValue)
		{
			return startValue * _model.value;
		}
	}

	public class PercentDecreaseFilter : BonusFilter
	{
		public override float Value(float startValue)
		{
			return startValue / _model.value;
		}
	}

	public class AdditionalFilter : BonusFilter
	{
		public override float Value(float startValue)
		{
			return startValue * _model.value;
		}
	}

	public class AbsoluteFilter : BonusFilter
	{
		public override float Value(float startValue)
		{
			return _model.value;
		}
	}
}