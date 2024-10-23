using System.Collections.Generic;
using System.Linq;
using ComixArea.Configuration;
using ComixArea.UI;
using ComixArea.Util;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using VContainer;

namespace ComixArea.Flow
{

    public class MainMenuSceneProvider : IMainMenuSceneProvider
    {
        private AsyncOperationHandle<SceneInstance> _loadedSceneOperation;
        private readonly List<LevelSO> _levels;
        private readonly AssetReference _mainGameModeScene;

        [Inject]
        public MainMenuSceneProvider(AssetReference gameUiScene, LevelsSO levels)
        {
            _mainGameModeScene = gameUiScene;
            _levels = levels.Levels.Where(l => l != null).ToList();
        }

        public async UniTask<(bool loadedSuccessfully, IMainMenu mainMenu)> LoadMainMenu()
        {
            await UnloadMainMenu();

            _loadedSceneOperation = Addressables.LoadSceneAsync(_mainGameModeScene, LoadSceneMode.Additive);
            SceneInstance sceneInstance = await _loadedSceneOperation;
            if (sceneInstance.Scene.TryGetRootGameObject<IMainMenu>(out var result))
            {
                //await result.InitAsync(_levels);
                return (true, result);
            }
            else
            {
                await UnloadMainMenu();
            }
            return (false, default);
        }

        public async UniTask UnloadMainMenu()
        {
            if (_loadedSceneOperation.IsValid())
            {
                await Addressables.UnloadSceneAsync(_loadedSceneOperation);
            }
        }
    }
}
