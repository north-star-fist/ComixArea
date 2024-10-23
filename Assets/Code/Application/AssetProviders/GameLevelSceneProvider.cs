using System.Collections.Generic;
using System.Linq;
using ComixArea.Configuration;
using ComixArea.Util;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using VContainer;

namespace ComixArea.Flow
{

    public class GameLevelSceneProvider : IGameLevelSceneProvider
    {
        private AsyncOperationHandle<SceneInstance> _loadedSceneOperation;
        private List<LevelSO> _levels;

        [Inject]
        public void Init(LevelsSO levels)
        {
            _levels = levels.Levels.Where(l => l != null).ToList();
        }

        public async UniTask UnloadGameEnvironment()
        {
            if (_loadedSceneOperation.IsValid())
            {
                await Addressables.UnloadSceneAsync(_loadedSceneOperation);
            }
        }

        private AssetReference GetSceneRef(int levelInd)
        {
            return _levels.Count > levelInd ? _levels[levelInd].LevelScene : null;
        }

        public async UniTask<(bool loaded, ILevelMap level)> LoadGameEnvironment(int levelIndex)
        {
            await UnloadGameEnvironment();
            AssetReference sceneRef = GetSceneRef(levelIndex);
            if (sceneRef == null)
            {
                return (false, null);
            }
            _loadedSceneOperation = Addressables.LoadSceneAsync(sceneRef, LoadSceneMode.Additive);
            SceneInstance sceneInstance = await _loadedSceneOperation;
            if (sceneInstance.Scene.IsValid() && sceneInstance.Scene.TryGetRootGameObject<ILevelMap>(out var levelMap))
            {
                return (true, levelMap);
            }
            return (false, null);
        }
    }
}
