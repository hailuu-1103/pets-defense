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

        private GameState gameState;
        private SignalBus signalBus;

        #endregion

        public UserTempData userTempData = new();

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
            JsonUtil.Save(this.userTempData, $"{FilePath}/Temp.json");
            Debug.Log($"Save data to file: {FilePath}/Temp.json");
        }
        public void ReadFromFile()
        {
            this.userTempData = JsonUtil.Load<UserTempData>($"{FilePath}/Temp.json");
            Debug.Log($"Load data from file {FilePath}/Temp.json successfully!");
        }
        private void SetUpTempData()
        {
            this.userTempData.gold  = this.gameState.goldCollected;
            this.userTempData.score = this.gameState.scoreCollected;
            for (var i = 0; i < this.enemyHolder.childCount; i++)
            {
                var enemy     = this.enemyHolder.GetChild(i);
                var enemyName = enemy.name.Replace("(Clone)", "").Trim();
                var enemyPos  = enemy.GetComponent<Transform>().position;
                this.userTempData.enemies.Add(new EnemyData { name = enemyName, horizontalPosition = enemyPos.x, verticalPosition = enemyPos.y });
            }
        }
        public void Start() { this.userTempData ??= new UserTempData(); }
    }
}