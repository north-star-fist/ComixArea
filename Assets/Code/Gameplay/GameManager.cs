using System.Threading;
using Cinemachine;
using ComixArea.Configuration;
using ComixArea.Input;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ComixArea
{
    /// <summary>
    /// Scene object that manages the game.
    /// </summary>
    public class GameManager : MonoBehaviour, IGameManager
    {
        [SerializeField]
        private CinemachineVirtualCameraBase _cinemachineCamera;

        public CinemachineVirtualCameraBase CinemachineCamera => _cinemachineCamera;

        public CinemachineConfiner CinemachineConfiner => _cinemachineCamera.GetComponent<CinemachineConfiner>();

        private ILevelMap _sceneContext;
        private GameObject _playerModel;


        public void Init(ILevelMap levelMap, PlayerInput characterPrefab, GameObject playerModel)
        {
            _sceneContext = levelMap;
            _playerModel = playerModel;
        }

        /// <summary>
        /// Starts new game with specified animal database.
        /// </summary>
        public async UniTask StartGameAsync(LevelSO level, CancellationToken cancellationToken)
        {
        }
    }
}
