namespace Installer
{
    using Manage;
    using Zenject;

    public class MainMenuInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.Bind<SaveLoadSystem>().FromComponentInHierarchy().AsCached();
        }
    }
}