using Cysharp.Threading.Tasks;

namespace ComixArea.Flow
{
    public interface IGameLevelSceneProvider
    {
        /// <summary>
        /// Loads level scene by it's index.
        /// </summary>
        public UniTask<(bool loaded, ILevelMap level)> LoadGameEnvironment(int levelIndex);

        /// <summary>
        /// Unloads level scene previously loaded.
        /// </summary>
        public UniTask UnloadGameEnvironment();
    }
}
