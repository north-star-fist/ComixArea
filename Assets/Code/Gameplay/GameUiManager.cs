using System;
using ComixArea.Configuration;
using ComixArea.Input;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VContainer;
using static ComixArea.Input.UIControl;

namespace ComixArea
{
    public class GameUiManager : MonoBehaviour, IGameUiManager, IUIActions
    {
        [Header("HUD")]
        [SerializeField] private Slider _healthSlider1;
        [SerializeField] private TMP_Text _nameText1;
        [SerializeField] private GameObject _player2Indicators;
        [SerializeField] private Slider _healthSlider2;
        [SerializeField] private TMP_Text _nameText2;

        [Header("Game Over")]
        [SerializeField] private GameObject _victoryPanel;
        [SerializeField] private GameObject _losePanel;

        [Header("Pause Menu")]
        [SerializeField] private Slider _masterSlider;
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Slider _sfxSlider;
        [SerializeField] private GameObject _pauseMenuPanel;
        [SerializeField] private Button _unpauseButton;
        [SerializeField] private Button _exitToMenuButton;

        [Inject]
        private ISettingsManager _settingsManager;

        private Settings _currentSettings;

        private bool _paused;
        private UIControl _uiControl;

        private IDisposable _h1Sub;
        private IDisposable _h2Sub;

        public Observable<Unit> OnExitToMainMenu => _exitToMenuButton.onClick.AsObservable();

        private void Start()
        {
            _currentSettings = _settingsManager.GetCurrentGameSettings();

            //Just for simplicity
            _masterSlider.value = _currentSettings.MasterVolume;
            _sfxSlider.value = _currentSettings.SfxVolume;
            _musicSlider.value = _currentSettings.MusicVolume;

            _masterSlider.onValueChanged.AsObservable().Subscribe(SetMasterVolume).AddTo(this);
            _sfxSlider.onValueChanged.AsObservable().Subscribe(SetSfxVolume).AddTo(this);
            _musicSlider.onValueChanged.AsObservable().Subscribe(SetMusicVolume).AddTo(this);

            _uiControl = new UIControl();
            _uiControl.Enable();
            _uiControl.UI.Enable();
            _uiControl.UI.AddCallbacks(this);

            _unpauseButton.onClick.AsObservable().Subscribe(_ => TogglePause()).AddTo(this);
            _exitToMenuButton.onClick.AsObservable().Subscribe(_ => TogglePause()).AddTo(this);
        }

        private void OnDestroy()
        {
            _uiControl.UI.RemoveCallbacks(this);
            _uiControl.UI.Disable();
            _uiControl.Disable();
            _uiControl.Dispose();
        }

        public void OnToggleMenu(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                TogglePause();
            }
        }


        private void TogglePause()
        {
            _paused = !_paused;
            _pauseMenuPanel.SetActive(_paused);
            Time.timeScale = _paused ? 0.0f : 1.0f;
        }

        private void SetMusicVolume(float volume)
        {
            Settings settings = GetCurrentGameSettings();
            settings.MusicVolume = volume;
            SetGameSettings(settings);
        }

        private void SetSfxVolume(float volume)
        {
            Settings settings = GetCurrentGameSettings();
            settings.SfxVolume = volume;
            SetGameSettings(settings);
        }

        private void SetMasterVolume(float volume)
        {
            Settings settings = GetCurrentGameSettings();
            settings.MasterVolume = volume;
            SetGameSettings(settings);
        }

        public Settings GetCurrentGameSettings()
        {
            if (_currentSettings == null)
            {
                return _settingsManager.GetCurrentGameSettings();
            }
            return _currentSettings;
        }

        public void SetGameSettings(Settings settings)
        {
            _settingsManager.ActivateSettings(settings);
            _settingsManager.SaveSettings(settings);
        }

        public void SetPlayer1HealthRx(Observable<float> h1, string name1)
        {
            if (_h1Sub != null)
            {
                _h1Sub.Dispose();
            }
            _h1Sub = h1.Subscribe(h => { _healthSlider1.value = h; }).AddTo(this);
            _nameText1.text = name1;
        }
        public void SetPlayer2HealthRx(Observable<float> h2, string name2)
        {
            _player2Indicators.SetActive(true);
            if (_h2Sub != null)
            {
                _h2Sub.Dispose();
            }
            _h2Sub = h2.Subscribe(h => { _healthSlider2.value = h; }).AddTo(this);
            _nameText2.text = name2;
        }

        public void HidePlayer2Health() => _player2Indicators.SetActive(false);

        public void ShowVictory() => _victoryPanel.SetActive(true);

        public void ShowFail() => _losePanel.SetActive(true);
    }
}
