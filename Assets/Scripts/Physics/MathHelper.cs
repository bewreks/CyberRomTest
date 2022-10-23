using UnityEngine;

namespace Physics
{
	public static class MathHelper
	{
		public static bool PointBetweenPoints(Vector3 a, Vector3 b, Vector3 testing)
		{
			var first  = (a - testing).sqrMagnitude;
			var second = (b - testing).sqrMagnitude;
			return first + second <= (a - b).sqrMagnitude;
		}

		public static bool IsRaysIntersects(Vector3     line1Dot1,
		                                    Vector3     line1Dot2,
		                                    Vector3     line2Dot1,
		                                    Vector3     line2Dot2,
		                                    out Vector3 position)
		{
			var vector1 = line1Dot2 - line1Dot1;
			var vector2 = line2Dot2 - line2Dot1;

			var matrix = new Matrix2X2();

			matrix.Initialize(vector1.y, vector2.y, vector1.x, vector2.x);
			var D = matrix.Determinant();

			if (D == 0)
			{
				position = Vector3.positiveInfinity;
				return false;
			}

			var c1 = -vector1.y * line1Dot1.x + vector1.x * line1Dot1.y;
			var c2 = -vector2.y * line2Dot1.x + vector2.x * line2Dot1.y;

			matrix.Initialize(-c1, -c2, vector1.x, vector2.x);
			var D1 = matrix.Determinant();

			matrix.Initialize(vector1.y, vector2.y, -c1, -c2);
			var D2 = matrix.Determinant();
			position = new Vector3(D1 / D, -D2 / D, 0);
			return true;
		}

		public static bool IsLinesIntersects(Vector3     line1Dot1,
		                                     Vector3     line1Dot2,
		                                     Vector3     line2Dot1,
		                                     Vector3     line2Dot2,
		                                     out Vector3 position)
		{
			if (IsRaysIntersects(line1Dot1, line1Dot2, line2Dot1, line2Dot2, out var testPoint))
			{
				if (testPoint.Equals(line2Dot1))
				{
					position = Vector3.zero;
					return false;
				}
				
				if (PointBetweenPoints(line1Dot1, line1Dot2, testPoint) &&
				    PointBetweenPoints(line2Dot1, line2Dot2, testPoint))
				{
					position = testPoint;
					return true;
				}
			}

			position = Vector3.zero;
			return false;
		}
	}
}