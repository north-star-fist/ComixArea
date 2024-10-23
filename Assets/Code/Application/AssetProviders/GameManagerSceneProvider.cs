using ComixArea.Util;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using VContainer;

namespace ComixArea.Flow
{

    public class GameManagerSceneProvider : IGameManagerSceneProvider
    {
        private AsyncOperationHandle<SceneInstance> _loadedSceneOperation;
        private readonly AssetReference _gameManagerScene;

        [Inject]
        public GameManagerSceneProvider(AssetReference gameManagerScene)
        {
            _gameManagerScene = gameManagerScene;
        }

        public async UniTask<(bool, IGameManager)> LoadGameManager()
        {
            await UnloadGameManager();

            _loadedSceneOperation = Addressables.LoadSceneAsync(_gameManagerScene, LoadSceneMode.Additive);
            SceneInstance sceneInstance = await _loadedSceneOperation;
            if (sceneInstance.Scene.TryGetRootGameObject<IGameManager>(out var result))
            {
                return (true, result);
            }
            else
            {
                await UnloadGameManager();
                return (false, default);
            }
        }

        public async UniTask UnloadGameManager()
        {
            if (_loadedSceneOperation.IsValid())
            {
                await Addressables.UnloadSceneAsync(_loadedSceneOperation);
            }
        }
    }
}
