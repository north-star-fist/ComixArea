using UnityEngine;
using VContainer;

namespace ComixArea.Configuration
{
    [CreateAssetMenu(fileName = "Levels Installer", menuName = "Comix Area/DI Installers/Levels")]
    public class LevelsInstaller : AScriptableInstaller
    {
        [SerializeField]
        private LevelsSO _levels;

        public override void Install(IContainerBuilder builder)
        {
            builder.RegisterInstance(_levels);
        }
    }
}
