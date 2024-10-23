using System;
using System.Collections.Generic;
using ComixArea.Configuration;
using ComixArea.StateMachine;
using ComixArea.UI;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using VContainer;

namespace ComixArea.Flow
{
    public class AppStateMenu : IState
    {
        private IAppFlow _appFlow;
        private IPlayerSetup _playerSetup;

        private readonly IList<LevelSO> _levels;
        private readonly IList<ACharacterSO> _characters;
        private readonly IMainMenuSceneProvider _uiSceneProvider;


        private IMainMenu _mainMenu;
        private int _currentLevel;
        private int _hero;

        private DisposableBag _disposables = new DisposableBag();

        [Inject]
        public void Init(IAppFlow appFlow, IPlayerSetup playerSetup)
        {
            _appFlow = appFlow;
            _playerSetup = playerSetup;
        }

        public AppStateMenu(IList<LevelSO> levels, IList<ACharacterSO> characters, IMainMenuSceneProvider uiSceneProvider)
        {
            _levels = levels;
            _uiSceneProvider = uiSceneProvider;
            _characters = characters;
        }

        public async UniTask OnEnterAsync()
        {
            _disposables.Dispose();
            _disposables = new();

            var loadResult = await _uiSceneProvider.LoadMainMenu();

            if (!loadResult.loadedSuccessfully)
            {
                throw new InvalidOperationException("Main Menu Scene Provider was not loaded");
            }
            _mainMenu = loadResult.mainMenu;
            _mainMenu.OnStart.Subscribe(_ => StartGameAsync().Forget()).AddTo(ref _disposables);
            _mainMenu.OnExit.Subscribe(_ => Application.Quit()).AddTo(ref _disposables);
            _mainMenu.OnLevelChosen.Subscribe(lvl => _currentLevel = lvl).AddTo(ref _disposables);
            _mainMenu.OnHeroChosen.Subscribe(hero => _hero = hero).AddTo(ref _disposables);
        }

        public async UniTask OnExitAsync()
        {
            await _uiSceneProvider.UnloadMainMenu();

            _disposables.Dispose();
        }

        public UniTask StartAsync()
        {
            // Do nothing because the game is launched via UI button
            return UniTask.CompletedTask;
        }

        private async UniTask StartGameAsync()
        {
            _playerSetup.SetPlayerModel(_characters[_hero].Model);
            await _appFlow.GoToStateAsync<AppStateGame>(SetupLevel);
        }

        private void SetupLevel(AppStateGame gameState)
        {
            gameState.SetUp(_currentLevel, _hero);
        }
    }
}
