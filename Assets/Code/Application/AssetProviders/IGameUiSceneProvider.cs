using Cysharp.Threading.Tasks;

namespace ComixArea.Flow
{
    public interface IGameUiSceneProvider
    {
        public UniTask<(bool loadedSuccessfully, IGameUiManager gameUi)> LoadGameUi();

        public UniTask UnloadGameUi();
    }
}
