using System;

namespace Player
{
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