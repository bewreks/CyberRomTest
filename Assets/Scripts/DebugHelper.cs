using UnityEngine;

public static class DebugHelper
{
	public static void DrawDebugRect(this Rect rect, Color color)
	{
		Debug.DrawLine(new Vector3(rect.xMin, rect.yMin),
		               new Vector3(rect.xMax, rect.yMin),
		               color, Time.fixedDeltaTime);

		Debug.DrawLine(new Vector3(rect.xMax, rect.yMin),
		               new Vector3(rect.xMax, rect.yMax),
		               color, Time.fixedDeltaTime);

		Debug.DrawLine(new Vector3(rect.xMax, rect.yMax),
		               new Vector3(rect.xMin, rect.yMax),
		               color, Time.fixedDeltaTime);

		Debug.DrawLine(new Vector3(rect.xMin, rect.yMax),
		               new Vector3(rect.xMin, rect.yMin),
		               color, Time.fixedDeltaTime);
	}
}