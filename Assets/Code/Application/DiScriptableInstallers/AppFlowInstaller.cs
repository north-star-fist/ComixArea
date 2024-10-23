using System.Collections.Generic;
using ComixArea.Flow;
using UnityEngine;
using VContainer;

namespace ComixArea.Configuration
{
    /// <summary>
    /// Installer that configures <see cref="IAppFlow"/> and registers it in DI container.
    /// </summary>
    [CreateAssetMenu(menuName = "Comix Area/DI Installers/AppFlow", fileName = "AppFlow Installer")]
    public class AppFlowInstaller : AScriptableInstaller
    {
        public override void Install(IContainerBuilder builder)
        {
            builder.Register<AppFlow>(Lifetime.Scoped).As<IAppFlow>();

            builder.RegisterBuildCallback(container =>
            {
                // Creating App State Machine when DI Context is ready
                var appFlow = container.Resolve<IAppFlow>();
                appFlow.RegisterState(new AppStateBoot());
                IList<LevelSO> levels = container.Resolve<LevelsSO>().Levels;
                IList<ACharacterSO> characters = container.Resolve<CharacterListSO>().Characters;
                IMainMenuSceneProvider menuSceneProvider = container.Resolve<IMainMenuSceneProvider>();
                AppStateMenu menuState = new AppStateMenu(levels, characters, menuSceneProvider);
                appFlow.RegisterState(menuState);
                IGameManagerSceneProvider gameManagerSceneProvider = container.Resolve<IGameManagerSceneProvider>();
                IGameLevelSceneProvider levelScenesProvider = container.Resolve<IGameLevelSceneProvider>();
                IGameUiSceneProvider uiSceneProvider = container.Resolve<IGameUiSceneProvider>();
                AppStateGame gameState = new AppStateGame(levels, gameManagerSceneProvider, levelScenesProvider, uiSceneProvider);
                appFlow.RegisterState(gameState);
            });
        }
    }
}
