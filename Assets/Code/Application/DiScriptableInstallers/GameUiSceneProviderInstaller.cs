using ComixArea.Flow;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;

namespace ComixArea.Configuration
{
    [CreateAssetMenu(
        fileName = "Game UI Scene Provider Installer",
        menuName = "Comix Area/DI Installers/Game UI Scene Provider"
    )]
    public class GameUiSceneProviderInstaller : AScriptableInstaller
    {
        [SerializeField, AssetReferenceUILabelRestriction(AddressablesTags.GameplayUiScene)]
        private AssetReference _gameUiScene;

        public override void Install(IContainerBuilder builder)
        {
            builder.Register<GameUiSceneProvider>(Lifetime.Scoped)
                .WithParameter(_gameUiScene)
                .As<IGameUiSceneProvider>();
        }
    }
}
