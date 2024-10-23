using Cysharp.Threading.Tasks;

namespace ComixArea.Flow
{
    public interface IGameManagerSceneProvider
    {
        public UniTask<(bool loadedSuccessfully, IGameManager gameManager)> LoadGameManager();

        public UniTask UnloadGameManager();
    }
}
