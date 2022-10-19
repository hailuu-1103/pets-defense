namespace Installer
{
    using GameFoundation.Scripts;
    using GameFoundation.Scripts.BlueprintFlow.BlueprintControlFlow;
    using GameFoundation.Scripts.BlueprintFlow.DebugBlueprint;
    using GameFoundation.Scripts.GameManager;
    using GameFoundation.Scripts.ScreenFlow.Managers;
    using LocalData;
    using Manage;
    using Zenject;

    public class PetsDefenseInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            GameFoundationInstaller.Install(this.Container);

            this.Container.Rebind<BlueprintReaderManager>().AsCached();

            // Scene Director
            this.Container.Bind<PetsDefenseSceneDirector>().AsSingle().NonLazy();
            this.Container.Rebind<SceneDirector>().FromResolveGetter<PetsDefenseSceneDirector>(data => data).AsCached();

            // Blue Print
            this.Container.Rebind<BlueprintConfig>().To<DebugBlueprintConfig>().AsCached();
            this.Container.Bind<PetsDefenseDebugBlueprintReaderManager>().AsCached();

            this.Container.Bind<UserLocalData>().FromResolveGetter<HandleLocalDataServices>
                (services => services.Load<UserLocalData>()).AsCached();

            GameSignalsInstaller.Install(this.Container);

            //Game State
            this.Container.Bind<GameState>().AsCached().NonLazy();
            
            
        }
    }
}