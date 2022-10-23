using System;
using Bonuses;
using UnityEngine;

namespace Player
{
	[Serializable]
	public class PlayerModel
	{
		private float      _width;
		public  ulong      Id            { get; private set; }
		public  bool       IsOnMainScene { get; private set; }
		public  byte       Score         { get; private set; }
		public  PlayerType Type          { get; private set; }

		public float Width
		{
			get => _sizeFilter.Value(_width);
			private set => _width = value;
		}

		public  float      Height        { get; private set; }
		public  Vector3    Left          { get; private set; }
		public  Vector3    Right         { get; private set; }

		private BonusFilter _sizeFilter = BonusFilterFactory.GetNoneFilter();
		private IDisposable _previousSizeBonus;

		public PlayerModel(ulong id, PlayerType type, byte startScore)
		{
			Id    = id;
			Type  = type;
			Score = startScore;
		}

		public void OnMainScene()
		{
			IsOnMainScene = true;
		}

		public void DecreaseScore()
		{
			Score--;
		}

		public void SetColliderSize(float width, float height)
		{
			Width  = width;
			Height = height;
			CalcPoints();
		}

		public void SetWidth(float width)
		{
			Width  = width;
			CalcPoints();
		}

		private void CalcPoints()
		{
			var halfWidth = Width / 2f;
			var height    = Type.GetSide() * Height;
			Left  = new Vector3(-halfWidth, -height);
			Right = new Vector3(halfWidth,  -height);
		}

		public void ApplyBonus(BonusModel bonus)
		{
			if (bonus.type == BonusTypes.Size)
			{
				BonusFilterFactory.Release(_sizeFilter);
				_sizeFilter = BonusFilterFactory.GetFilter(bonus);
				CalcPoints();
				_previousSizeBonus = bonus.StartTimer(() =>
				{
					BonusFilterFactory.Release(_sizeFilter);
					_sizeFilter = BonusFilterFactory.GetNoneFilter();
					_previousSizeBonus?.Dispose();
					_previousSizeBonus = null;
					CalcPoints();
				});
			}
		}
	}

	public enum PlayerType
	{
		NotSet = -1,
		Bottom = 0,
		Top    = 1,
	}
}