using System;

namespace Bonuses
{
	[Serializable]
	public class BonusModel
	{
		public uint              id; 
		public float             value;
		public float             time;
		public BonusTypes        type;
		public BonusValueType    valueType;
		public BonusRelationship relationship;
		public BonusView         viewPrefab;
	}

	public enum BonusTypes
	{
		Size,
		Speed,
	}

	public enum BonusValueType
	{
		Absolute,
		PercentIncrease,
		PercentDecrease,
		Additional
	}

	public enum BonusRelationship
	{
		Player,
		Enemy,
		Ball
	}
}