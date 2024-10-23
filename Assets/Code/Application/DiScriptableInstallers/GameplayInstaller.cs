using ComixArea.Flow;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;

namespace ComixArea.Configuration
{
    [CreateAssetMenu(
        fileName = "GameManager Scene Provider Installer",
        menuName = "Comix Area/DI Installers/GameManager Scene Provider"
    )]
    public class GameplayInstaller : AScriptableInstaller
    {
        [SerializeField, AssetReferenceUILabelRestriction(AddressablesTags.GameplayManagerScene)]
        private AssetReference _gameManagerScene;

        public override void Install(IContainerBuilder builder)
        {
            builder.Register<GameManagerSceneProvider>(Lifetime.Scoped)
                .WithParameter(_gameManagerScene)
                .As<IGameManagerSceneProvider>();
        }
    }
}
