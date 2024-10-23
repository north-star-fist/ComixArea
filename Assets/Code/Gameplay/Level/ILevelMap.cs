using ComixArea.Navigation;
using UnityEngine;
using static ComixArea.LevelMap;

namespace ComixArea
{
    public interface ILevelMap
    {
        /// <summary>
        /// Player spawn position.
        /// </summary>
        public Vector3 PlayerSpawnPosition { get; }

        /// <summary>
        /// Player initial direction.
        /// </summary>
        HorizontalDirection PlayerSpawnLookDirection { get; }

        /// <summary>
        /// Enemy setups.
        /// </summary>
        public EnemySpawnPoint[] EnemySpawnPositions { get; }

        /// <summary>
        /// Parent transform for all enemy game objects.
        /// </summary>
        public Transform EnemiesParent { get; }

        /// <summary>
        /// Parent transform for player game object.
        /// </summary>
        public Transform PlayerParent { get; }

        /// <summary>
        /// Level bounds.
        /// </summary>
        public BoxCollider LevelCameraBounds { get; }
    }
}
