using ComixArea.UI;
using Cysharp.Threading.Tasks;

namespace ComixArea.Flow
{
    public interface IMainMenuSceneProvider
    {
        public UniTask<(bool loadedSuccessfully, IMainMenu mainMenu)> LoadMainMenu();

        public UniTask UnloadMainMenu();
    }
}
