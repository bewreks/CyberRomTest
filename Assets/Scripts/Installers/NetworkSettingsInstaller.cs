using System;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Installers
{
    [CreateAssetMenu(fileName = "NetworkSettingsInstaller", menuName = "Installers/NetworkSettingsInstaller")]
    public class NetworkSettingsInstaller : ScriptableObjectInstaller<NetworkSettingsInstaller>
    {
        [SerializeField] private NetworkSettings networkSettings;
        [SerializeField] private GameObject      networkPrefab;
        
        public override void InstallBindings()
        {
            Instantiate(networkPrefab);
            Container.Bind<NetworkManager>().FromInstance(NetworkManager.Singleton).AsSingle();
            Container.Bind<NetworkSettings>().FromInstance(networkSettings).AsSingle();
        }
    }

    [Serializable]
    public class NetworkSettings
    {
        [SerializeField] private int _playersCount = 2;

        public int PlayersCount => _playersCount;
    }
}