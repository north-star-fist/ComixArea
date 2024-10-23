using ComixArea.Flow;
using UnityEngine;
using VContainer;

namespace ComixArea.Configuration
{
    [CreateAssetMenu(fileName = "Level Provider Installer", menuName = "Comix Area/DI Installers/Level Provider")]
    public class LevelScenesInstaller : AScriptableInstaller
    {
        public override void Install(IContainerBuilder builder)
        {
            builder.Register<GameLevelSceneProvider>(Lifetime.Scoped)
                .As<IGameLevelSceneProvider>();
        }
    }
}
