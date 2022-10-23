using System;
using UnityEngine;

namespace Player
{
	public class PlayerModel
	{
		public ulong      Id            { get; private set; }
		public bool       IsOnMainScene { get; private set; }
		public byte       Score         { get; private set; }
		public PlayerType Type          { get; private set; }
		public float      Width         { get; private set; }
		public float      Height        { get; private set; }
		public Vector3    Left          { get; private set; }
		public Vector3    Right         { get; private set; }

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
	}

	public enum PlayerType
	{
		NotSet = -1,
		Bottom = 0,
		Top    = 1,
	}

	public static class PlayerHelper
	{
		public static PlayerType OppositeSide(this PlayerType type)
		{
			return type switch
			       {
				       PlayerType.Bottom => PlayerType.Top,
				       PlayerType.Top    => PlayerType.Bottom,
				       _                 => throw new ArgumentOutOfRangeException(nameof(type), type, null)
			       };
		}

		public static int GetSide(this PlayerType type)
		{
			return type switch
			       {
				       PlayerType.Bottom => -1,
				       PlayerType.Top    => 1,
				       _                 => throw new ArgumentOutOfRangeException(nameof(type), type, null)
			       };
		}
	}
}