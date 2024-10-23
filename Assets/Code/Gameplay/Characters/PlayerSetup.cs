using ComixArea.Creatures;
using UnityEngine;

namespace ComixArea
{
    public class PlayerSetup : IPlayerSetup
    {
        private GameObject _playerModel;

        private readonly Fighter _playerPrefab;
        private readonly Fighter _botPrefab;

        public GameObject PlayerModel => _playerModel;

        public Fighter PlayerPrefab => _playerPrefab;

        public Fighter BotPrefab => _botPrefab;

        public PlayerSetup(Fighter characterPrefab, Fighter botPrefab)
        {
            _playerPrefab = characterPrefab;
            _botPrefab = botPrefab;
        }

        public void SetPlayerModel(GameObject playerModelPrefab)
        {
            _playerModel = playerModelPrefab;
        }
    }
}
