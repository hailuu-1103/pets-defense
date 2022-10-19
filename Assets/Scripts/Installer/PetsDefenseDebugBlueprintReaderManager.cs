namespace Installer
{
    using GameFoundation.Scripts.BlueprintFlow.BlueprintControlFlow;
    using GameFoundation.Scripts.GameManager;
    using GameFoundation.Scripts.Network.WebService;
    using GameFoundation.Scripts.Utilities.LogService;
    using Zenject;

    public class PetsDefenseDebugBlueprintReaderManager : BlueprintReaderManager 
    {
        private readonly PreProcessBlueprintMobile preProcessBlueprintMobile;
        public PetsDefenseDebugBlueprintReaderManager(SignalBus signalBus, ILogService logService, DiContainer diContainer, PreProcessBlueprintMobile preProcessBlueprintMobile,GameFoundationLocalData localData, HandleLocalDataServices handleLocalDataServices, IHttpService httpService, BlueprintConfig blueprintConfig) : base(signalBus, logService, diContainer, localData, handleLocalDataServices, httpService, blueprintConfig)
        {
            this.preProcessBlueprintMobile = preProcessBlueprintMobile;
        }
        protected override bool IsLoadLocalBlueprint(string url, string hash) { return true; }
        public override async void LoadBlueprint(string url, string hash = "test")
        {
            await this.preProcessBlueprintMobile.LoadStreamAsset();
            base.LoadBlueprint(url, hash);
        }
    }
}