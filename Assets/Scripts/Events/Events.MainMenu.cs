using System;

namespace Events
{
	public static partial class Events
	{
		public static event Action         OnHostButtonClick;
		public static event Action<string> OnClientButtonClick;
		public static event Action         OnShutdownButtonClick;

		public static void StartShutdownInvoke()
		{
			OnShutdownButtonClick?.Invoke();
		}

		public static void StartClientInvoke(string ip)
		{
			OnClientButtonClick?.Invoke(ip);
		}

		public static void StartHostInvoke()
		{
			OnHostButtonClick?.Invoke();
		}
	}
}