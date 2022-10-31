namespace Manage
{
    using GameData;
    using Signals;
    using UnityEngine;
    using Utils;
    using Zenject;

    public class SaveLoadSystem : MonoBehaviour
    {
        [SerializeField] private Transform enemyHolder;

        #region Zenject

        public GameState gameState;
        private SignalBus signalBus;

        #endregion


        private const string       FilePath = "Assets/StreamingAssets/TempData";

        [Inject]
        public void Construct(GameState state, SignalBus signalBus)
        {
            this.gameState = state;
            this.signalBus = signalBus;
        }
        public void SaveToFile()
        {
            this.SetUpTempData();
            JsonUtil.Save(this.gameState, $"{FilePath}/Temp.json");
            Debug.Log($"Save data to file: {FilePath}/Temp.json");
        }
        public void ReadFromFile()
        {
            this.gameState = JsonUtil.Load<GameState>($"{FilePath}/Temp.json");
            Debug.Log($"Load data from file {FilePath}/Temp.json successfully!");
        }
        private void SetUpTempData()
        {
            for (var i = 0; i < this.enemyHolder.childCount; i++)
            {
                var enemy     = this.enemyHolder.GetChild(i);
                var enemyName = enemy.name.Replace("(Clone)", "").Trim();
                var enemyPos  = enemy.GetComponent<Transform>().position;
                this.gameState.enemies.Add(new EnemyData { name = enemyName, horizontalPosition = enemyPos.x, verticalPosition = enemyPos.y });
            }
        }
        public void Start() { this.gameState ??= new GameState(); }
    }
}