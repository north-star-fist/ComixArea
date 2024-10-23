using System.Threading;
using Cinemachine;
using ComixArea.Configuration;
using Cysharp.Threading.Tasks;

namespace ComixArea
{
    public interface IGameManager
    {
        UniTask StartGameAsync(LevelSO level, CancellationToken cancellationToken);

        public CinemachineVirtualCameraBase CinemachineCamera { get; }
        public CinemachineConfiner CinemachineConfiner { get; }
    }
}
