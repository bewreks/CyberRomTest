using Unity.Netcode;
using UnityEngine;

namespace Bonuses
{
	[RequireComponent(typeof(NetworkObject))]
	public class BonusView : NetworkBehaviour
	{
		private NetworkObject _networkObject;


		private void Awake()
		{
			_networkObject = GetComponent<NetworkObject>();
		}

		public void Spawn()
		{
			if (_networkObject)
				_networkObject.Spawn();
		}

		public void Despawn()
		{
			if (_networkObject)
				_networkObject.Despawn();
		}
	}
}