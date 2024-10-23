using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ComixArea.AI;
using ComixArea.Configuration;
using ComixArea.Creatures;
using ComixArea.StateMachine;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using VContainer;

namespace ComixArea.Flow
{
    public class AppStateGame : IState
    {
        private readonly IList<LevelSO> _levels;
        private readonly IGameManagerSceneProvider _gameManagerSceneProvider;
        private readonly IGameLevelSceneProvider _levelsProvider;
        private readonly IGameUiSceneProvider _uiSceneProvider;

        private IAppFlow _appFlow;
        private IPlayerSetup _playerSetup;
        private CharacterListSO _characters;
        private IGameManager _gameManager;
        private IGameUiManager _uiManager;
        private int _currentLevel;
        private int _currentHero;
        private int _loadedLevel = -1;

        CancellationTokenSource _gameCancellationTokenSource;
        private DisposableBag _disposables = new DisposableBag();

        private ReactiveProperty<int> _botsSpawned = new ReactiveProperty<int>(0);

        // Fighter which health is shown at upper right corner.
        private Fighter _fighter2;

        [Inject]
        public void Init(IAppFlow appFlow, IPlayerSetup playerSetup, CharacterListSO characters)
        {
            _appFlow = appFlow;
            _playerSetup = playerSetup;
            _characters = characters;
        }

        public AppStateGame(
            IList<LevelSO> levels,
            IGameManagerSceneProvider gameSceneProvider,
            IGameLevelSceneProvider levelsProvider,
            IGameUiSceneProvider uiSceneProvider
        )
        {
            _levels = levels;
            _gameManagerSceneProvider = gameSceneProvider;
            _levelsProvider = levelsProvider;
            _uiSceneProvider = uiSceneProvider;
        }

        public void SetUp(int currentLevel, int hero)
        {
            _currentLevel = currentLevel;
            _currentHero = hero;
        }

        public async UniTask OnEnterAsync()
        {
            _botsSpawned.Value = 0;
            _disposables.Dispose();
            _disposables = new();
            (var gmLoadResult, var uiLoadResult, var lvl) = await UniTask.WhenAll(
                _gameManagerSceneProvider.LoadGameManager(),
                _uiSceneProvider.LoadGameUi(),
                _levelsProvider.LoadGameEnvironment(_currentLevel)
            );

            if (!lvl.loaded)
            {
                throw new InvalidOperationException(
                    $"Level {_currentLevel} was not loaded! Maybe there is no {nameof(LevelMap)} object."
                );
            }
            if (!gmLoadResult.loadedSuccessfully)
            {
                throw new InvalidOperationException("Game manager scene was not loaded!");
            }
            if (!uiLoadResult.loadedSuccessfully)
            {
                throw new InvalidOperationException("Levels Provider was not loaded");
            }
            _gameManager = gmLoadResult.gameManager;
            _gameManager.CinemachineConfiner.m_BoundingVolume = lvl.level.LevelCameraBounds;
            var player = spawnPlayer(lvl.level);

            _uiManager = uiLoadResult.gameUi;
            _uiManager.SetPlayer1HealthRx(player.Health.CurrentHealth.Select(h => h / player.Health.MaxHealth), "Player 1");
            _uiManager.OnExitToMainMenu.Subscribe(ExitToMainMenu).AddTo(ref _disposables);
            spawnBots(lvl.level, player);
            _botsSpawned.Where(botsNum => botsNum == 0).Subscribe(_ => _uiManager.ShowVictory()).AddTo(ref _disposables);
            player.Health.OnDamage.Where(damage => damage.IsFatal).Subscribe(_ => _uiManager.ShowFail()).AddTo(ref _disposables);
            Time.timeScale = 1f;

            return;

            Fighter spawnPlayer(ILevelMap level)
            {
                var player = GameObject.Instantiate(
                    _playerSetup.PlayerPrefab,
                    lvl.level.PlayerSpawnPosition,
                    Quaternion.identity,
                    lvl.level.PlayerParent
                );
                player.SetModel(GameObject.Instantiate(_playerSetup.PlayerModel), lvl.level.PlayerSpawnLookDirection);
                _gameManager.CinemachineCamera.LookAt = player.transform;
                _gameManager.CinemachineCamera.Follow = player.transform;
                return player;
            }

            void spawnBots(ILevelMap level, Fighter player1)
            {
                int charCount = _currentHero + 1;
                if (lvl.level.EnemySpawnPositions == null)
                {
                    return;
                }
                foreach (var spawnPoint in lvl.level.EnemySpawnPositions)
                {
                    var bot = GameObject.Instantiate(
                        _playerSetup.BotPrefab,
                        spawnPoint.Transform.position,
                        Quaternion.identity,
                        lvl.level.EnemiesParent
                    );
                    charCount = charCount >= _characters.Characters.Length ? 0 : charCount;
                    ACharacterSO character = _characters.Characters[charCount++];
                    GameObject botModelPrefab = character.Model;
                    var botMdel = GameObject.Instantiate(botModelPrefab);
                    bool rightDir = Vector3.Dot(Vector3.right, spawnPoint.Transform.rotation * Vector3.forward) >= 0;
                    Navigation.HorizontalDirection botDirection = rightDir
                        ? Navigation.HorizontalDirection.Right
                        : Navigation.HorizontalDirection.Left;
                    bot.SetModel(botMdel, botDirection);
                    bot.Name = character.Name;
                    bot.Health.OnDamage.Where(d => d.IsFatal).Subscribe(_ => _botsSpawned.Value--).AddTo(ref _disposables);
                    if (bot.TryGetComponent<PatrolingFighterAiController>(out var patrolBot))
                    {
                        patrolBot.SetWayPoints(spawnPoint.WayPoints.Select(t => t.position).ToArray());
                    }
                    if (bot.TryGetComponent<BotAiController>(out var botController))
                    {
                        botController.CurrentEnemyRx.Where(enemy => enemy == player1.gameObject)
                            .Subscribe(_ =>
                            {
                                _uiManager.SetPlayer2HealthRx(
                                    bot.Health.CurrentHealth.Select(h => h / bot.Health.MaxHealth),
                                    bot.Name
                                );
                                _fighter2 = bot;
                                botController.CurrentEnemyRx.Where(enemy => enemy == null && bot == _fighter2)
                                    .Subscribe(_ => _uiManager.HidePlayer2Health()).AddTo(ref _disposables);
                            })
                            .AddTo(bot);
                    }

                    _botsSpawned.Value++;
                }
            }
        }

        public async UniTask OnExitAsync()
        {
            await UniTask.WhenAll(
                _gameManagerSceneProvider.UnloadGameManager(),
                _levelsProvider.UnloadGameEnvironment(),
                _uiSceneProvider.UnloadGameUi()
            );
            _loadedLevel = -1;
            _disposables.Dispose();
        }

        public async UniTask StartAsync()
        {
            if (_gameCancellationTokenSource != null)
            {
                _gameCancellationTokenSource.Cancel();
                _gameCancellationTokenSource.Dispose();
            }
            _gameCancellationTokenSource = new CancellationTokenSource();

            _loadedLevel = _currentLevel;
            await _gameManager.StartGameAsync(_levels[_currentLevel], _gameCancellationTokenSource.Token);
        }

        private void ExitToMainMenu(Unit _)
        {
            _appFlow.GoToStateAsync<AppStateMenu>();
        }
    }
}
