namespace GameData
{
    using System;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.AssetLibrary;
    using Manage;
    using Signals;
    using UnityEngine;
    using Utils;
    using Zenject;

    public class EnemySpawner : MonoBehaviour
    {
        private SignalBus      signalBus;
        private IGameAssets    gameAssets;
        private SaveLoadSystem saveLoadSystem;

        private UserTempData userTempData;
        private string       Path = "Assets/StreamingAssets/TempData/Temp.json";
        [Inject]
        private void Construct(SignalBus signal, IGameAssets assets, SaveLoadSystem system)
        {
            this.signalBus      = signal;
            this.gameAssets     = assets;
            this.saveLoadSystem = system;
        }
        public async void Spawn()
        {
            this.userTempData = JsonUtil.Load<UserTempData>(this.Path);
            foreach (var enemyData in this.userTempData.enemies)
            {
                var enemy = Instantiate(await this.gameAssets.LoadAssetAsync<GameObject>(enemyData.name), 
                    new Vector3(enemyData.horizontalPosition, enemyData.verticalPosition, 0), 
                    Quaternion.identity,
                    this.transform);
            }
        }
    }
}