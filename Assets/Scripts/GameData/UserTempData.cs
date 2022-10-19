namespace GameData
{
    using System.Collections.Generic;

    public class UserTempData
    {
        public int             gold;
        public int             score;
        public List<TowerData> towers = new();
        public List<EnemyData> enemies = new();
    }
}