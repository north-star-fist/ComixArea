using R3;

namespace ComixArea.UI
{
    public interface IMainMenu
    {
        public Observable<Unit> OnStart { get; }

        public Observable<Unit> OnExit { get; }

        public Observable<int> OnLevelChosen { get; }

        public Observable<int> OnHeroChosen { get; }
    }
}
