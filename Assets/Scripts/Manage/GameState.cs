namespace Manage
{
    using Zenject;

    public class GameState : IInitializable
    {
        public int scoreCollected;
        public int goldCollected;
        public void Initialize()
        {
            this.scoreCollected = 0;
            this.goldCollected  = 100;
        }
    }
}