using UnityEngine;

namespace CameraTools
{
	public class CameraHelper
	{
		public static Rect WorldCameraRect(float cameraSize)
		{
			var halfWidth  = HalfWidth(cameraSize);
			return new Rect(-halfWidth, -cameraSize, halfWidth * 2, cameraSize * 2);
		}

		public static float HalfWidth(float cameraSize)
		{
			return 1.77f * cameraSize;
		}

		public static Vector3 ScreenToWorldWithLockByX(Camera camera, float cameraSize)
		{
			var screenToWorldPoint = camera.ScreenToWorldPoint(Input.mousePosition);

			var sign = Mathf.Sign(screenToWorldPoint.x);
			screenToWorldPoint.x = Mathf.Min(HalfWidth(cameraSize), Mathf.Abs(screenToWorldPoint.x)) * sign;
			return screenToWorldPoint;
		}
	}
}