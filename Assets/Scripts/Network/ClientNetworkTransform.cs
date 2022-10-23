using Unity.Netcode.Components;

namespace Network
{
	
	/*
	 * В этой демке мы доверяем клиенту, хоть и не должны
	 */
	public class ClientNetworkTransform : NetworkTransform
	{
		protected override bool OnIsServerAuthoritative()
		{
			return false;
		}
	}
}