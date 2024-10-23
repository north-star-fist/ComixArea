using System;
using ComixArea.Navigation;
using UnityEngine;

namespace ComixArea
{
    public class LevelMap : MonoBehaviour, ILevelMap
    {
        [SerializeField] private Transform _playerSpawnPoint;
        [SerializeField] private HorizontalDirection _playerSpawnLookDirection = HorizontalDirection.Right;
        [SerializeField] private Transform _playerParent;
        [SerializeField] private EnemySpawnPoint[] _enemySpawnPoints;
        [SerializeField] private Transform _enemiesParent;
        [SerializeField] private BoxCollider _levelCameraBounds;

        public Vector3 PlayerSpawnPosition => _playerSpawnPoint.position;

        public EnemySpawnPoint[] EnemySpawnPositions
        {
            get
            {
                return _enemySpawnPoints;
            }
        }

        public Transform EnemiesParent => _enemiesParent;

        public Transform PlayerParent => _playerParent;

        public BoxCollider LevelCameraBounds => _levelCameraBounds;

        public HorizontalDirection PlayerSpawnLookDirection => _playerSpawnLookDirection;

        [Serializable]
        public class EnemySpawnPoint
        {
            public Transform Transform;
            public Transform[] WayPoints;
        }
    }
}
