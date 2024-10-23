using ComixArea.Creatures;
using UnityEngine;

namespace ComixArea
{
    public interface IPlayerSetup
    {
        public Fighter PlayerPrefab { get; }

        public Fighter BotPrefab { get; }

        public GameObject PlayerModel { get; }

        public void SetPlayerModel(GameObject playerModelPrefab);
    }
}
