using ComixArea.Configuration;
using ComixArea.StateMachine;
using Cysharp.Threading.Tasks;
using VContainer;

namespace ComixArea.Flow
{
    public class AppStateBoot : IState
    {
        private IAppFlow _appStateService;
        private ISettingsManager _settingsManager;

        [Inject]
        public void Construct(IAppFlow appStateService, ISettingsManager settings)
        {
            _appStateService = appStateService;
            _settingsManager = settings;
        }

        public UniTask OnEnterAsync() => UniTask.CompletedTask;

        public UniTask OnExitAsync() => UniTask.CompletedTask;

        public async UniTask StartAsync()
        {
            _settingsManager.ActivateSettings(_settingsManager.GetCurrentGameSettings());
            await _appStateService.GoToStateAsync<AppStateMenu>(null);
        }
    }
}
