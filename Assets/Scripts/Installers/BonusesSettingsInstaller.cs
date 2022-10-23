using System;
using System.Collections.Generic;
using System.Linq;
using Bonuses;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Installers
{
    [CreateAssetMenu(fileName = "BonusesSettingsInstaller", menuName = "Installers/BonusesSettingsInstaller")]
    public class BonusesSettingsInstaller : ScriptableObjectInstaller<BonusesSettingsInstaller>
    {
        [SerializeField] private BonusesSettings _bonusesSettings;
    
        public override void InstallBindings()
        {
            _bonusesSettings.Prepare();
            Container.Bind<BonusesSettings>().FromInstance(_bonusesSettings).AsSingle();
        }
    }
    
    [Serializable]
    public class BonusesSettings
    {
        [SerializeField] private BonusModel[] bonuses;

        private Dictionary<uint, BonusModel> _bonusesMap;

        public void Prepare()
        {
            _bonusesMap = bonuses.ToDictionary(model => model.id, model => model);
        }
        
        public BonusModel GetBonus(uint id)
        {
            return _bonusesMap[id];
        }
        
        public BonusModel GetRandomBonus()
        {
            return bonuses[Random.Range(0, bonuses.Length - 1)];
        }
    }
}