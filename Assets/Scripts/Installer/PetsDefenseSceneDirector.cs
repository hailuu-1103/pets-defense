namespace Installer
{
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.ScreenFlow.Managers;
    using Zenject;
    
    public static class SceneName
    {
        public const string Loading = "LoadingScene";
        public const string Home    = "HomeScene";
        public const string Level   = "LevelScene";
        public const string Game    = "GameScene";
    }

    public class PetsDefenseSceneDirector : SceneDirector
    {
        public PetsDefenseSceneDirector(SignalBus signalBus, IGameAssets gameAssets) : base(signalBus, gameAssets)
        {
        }
        #region shortcut

        public async void LoadLoadingScene() { await this.LoadSingleSceneAsync(SceneName.Loading); }
        public async void LoadHomeScene()    { await this.LoadSingleSceneAsync(SceneName.Home); }
        public async void LoadLevelScene()   { await this.LoadSingleSceneAsync(SceneName.Level); }
        public async void LoadGameScene()    { await this.LoadSingleSceneAsync(SceneName.Game); }

        #endregion
    }
}