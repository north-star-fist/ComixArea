using R3;

namespace ComixArea
{
    public interface IGameUiManager
    {
        public Observable<Unit> OnExitToMainMenu { get; }

        public void SetPlayer1HealthRx(Observable<float> h1Rx, string fighterName);
        public void SetPlayer2HealthRx(Observable<float> h2Rx, string fighterName);
        public void HidePlayer2Health();

        public void ShowVictory();
        public void ShowFail();
    }
}
