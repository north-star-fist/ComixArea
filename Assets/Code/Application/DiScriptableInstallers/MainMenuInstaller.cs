using ComixArea.Flow;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;

namespace ComixArea.Configuration
{
    [CreateAssetMenu(
        fileName = "Main Menu Scene Provider Installer",
        menuName = "Comix Area/DI Installers/Main Menu Scene Provider"
    )]
    public class MainMenuInstaller : AScriptableInstaller
    {
        [SerializeField, AssetReferenceUILabelRestriction(AddressablesTags.MainMenuScene)]
        private AssetReference _mainMenuScene;

        public override void Install(IContainerBuilder builder)
        {
            builder.Register<MainMenuSceneProvider>(Lifetime.Scoped)
                .WithParameter(_mainMenuScene)
                .As<IMainMenuSceneProvider>();
        }
    }
}
