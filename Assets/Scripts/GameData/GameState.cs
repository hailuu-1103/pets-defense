namespace Manage
{
    using System.Collections.Generic;
    using GameData;
    using Zenject;

    public class GameState : IInitializable
    {
        public float           wave;
        public int             scoreCollected;
        public int             goldCollected;
        public List<TowerData> towers;
        public List<EnemyData> enemies;
        public void Initialize()
        {
            this.wave           = 0;
            this.scoreCollected = 0;
            this.goldCollected  = 100;
            this.towers         = new List<TowerData>();
            this.enemies        = new List<EnemyData>();
        }
    }
}