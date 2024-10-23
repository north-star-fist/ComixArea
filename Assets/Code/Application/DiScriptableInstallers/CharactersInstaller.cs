using ComixArea.Creatures;
using UnityEngine;
using VContainer;

namespace ComixArea.Configuration
{
    [CreateAssetMenu(fileName = "Characters Installer", menuName = "Comix Area/DI Installers/Characters")]
    public class CharactersInstaller : AScriptableInstaller
    {
        [SerializeField]
        private Fighter _playerPrefab;

        [SerializeField]
        private Fighter _botPrefab;

        [SerializeField]
        private CharacterListSO _characters;

        public override void Install(IContainerBuilder builder)
        {
            builder.RegisterInstance(_characters);
            PlayerSetup playerSetup = new PlayerSetup(_playerPrefab, _botPrefab);
            builder.RegisterInstance(playerSetup).As<IPlayerSetup>();
        }
    }
}
