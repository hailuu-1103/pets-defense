namespace GameData
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.AssetLibrary;
    using Manage;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using Utils;
    using Zenject;

    public class EnemySpawner : MonoBehaviour
    {
        private SignalBus      signalBus;
        private IGameAssets    gameAssets;
        private SaveLoadSystem saveLoadSystem;

        private          GameState gameState;
        private readonly string    Path = "Assets/StreamingAssets/TempData/Temp.json";
        [Inject]
        private void Construct(SignalBus signal, IGameAssets assets, SaveLoadSystem system, GameState state)
        {
            this.signalBus      = signal;
            this.gameAssets     = assets;
            this.saveLoadSystem = system;
            this.gameState      = state;
        }
        public async void Spawn()
        {
            this.gameState = JsonUtil.Load<GameState>(this.Path);
            foreach (var enemyData in this.gameState.enemies)
            {
                var enemy = Instantiate(await this.gameAssets.LoadAssetAsync<GameObject>(enemyData.name), 
                    new Vector3(enemyData.horizontalPosition, enemyData.verticalPosition, 0), 
                    Quaternion.identity,
                    this.transform);
            }
        }
    }
}