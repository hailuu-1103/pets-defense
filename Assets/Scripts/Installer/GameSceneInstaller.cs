namespace Installer
{
    using Manage;
    using Zenject;

    public class GameSceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.Bind<SaveLoadSystem>().FromComponentInHierarchy().AsCached();
            this.Container.BindInterfacesAndSelfTo<GameState>().AsCached();
            this.Container.Bind<SpawnPoint>().FromComponentInHierarchy().AsCached();
        }
    }
}