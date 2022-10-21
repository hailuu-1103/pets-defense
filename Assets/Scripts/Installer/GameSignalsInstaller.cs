namespace Installer
{
    using Signals;
    using Zenject;

    public class GameSignalsInstaller : Installer<GameSignalsInstaller>
    {
        public override void InstallBindings()
        {
            this.Container.DeclareSignal<NextWaveSignal>();
            this.Container.DeclareSignal<LoadGameSignal>();
        }
    }
}