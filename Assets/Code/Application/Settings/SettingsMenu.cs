using System;
using System.Collections.Generic;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace ComixArea.Configuration
{

    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField]
        private Slider _masterSlider;
        [SerializeField]
        private Slider _musicSlider;
        [SerializeField]
        private Slider _sfxSlider;
        [SerializeField]
        private TMP_Dropdown _resolutionsDropdown;
        [SerializeField]
        private Toggle _fullScreenToggle;
        [SerializeField]
        private TMP_Dropdown _qualityDropdown;
        [SerializeField]
        private bool _hideSameResolutionsWithLowRefreshRate = true;

        [Inject]
        private ISettingsManager _settingsManager;

        Resolution[] _resolutions;

        private Settings _currentSettings;


        void Start()
        {
            _currentSettings = _settingsManager.GetCurrentGameSettings();

            InitScreenResolutions();
            InitQualityDropdown();

            //Just for simplicity
            _masterSlider.value = _currentSettings.MasterVolume;
            _sfxSlider.value = _currentSettings.SfxVolume;
            _musicSlider.value = _currentSettings.MusicVolume;

            _fullScreenToggle.isOn = Screen.fullScreen;

            _masterSlider.onValueChanged.AsObservable().Subscribe(SetMasterVolume).AddTo(this);
            _sfxSlider.onValueChanged.AsObservable().Subscribe(SetSfxVolume).AddTo(this);
            _musicSlider.onValueChanged.AsObservable().Subscribe(SetMusicVolume).AddTo(this);
            _resolutionsDropdown.onValueChanged.AsObservable().Subscribe(SetResolution).AddTo(this);
            _fullScreenToggle.onValueChanged.AsObservable().Subscribe(SetFullScreen).AddTo(this);
            _qualityDropdown.onValueChanged.AsObservable().Subscribe(SetQualityLevel).AddTo(this);
        }

        public void SetMusicVolume(float volume)
        {
            Settings settings = GetCurrentGameSettings();
            settings.MusicVolume = volume;
            SetGameSettings(settings);
        }

        public void SetSfxVolume(float volume)
        {
            Settings settings = GetCurrentGameSettings();
            settings.SfxVolume = volume;
            SetGameSettings(settings);
        }

        public void SetMasterVolume(float volume)
        {
            Settings settings = GetCurrentGameSettings();
            settings.MasterVolume = volume;
            SetGameSettings(settings);
        }

        public void SetQualityLevel(int level)
        {
            Settings settings = GetCurrentGameSettings();
            settings.GraphicsQuality = level;
            SetGameSettings(settings);
        }

        public void SetFullScreen(bool fullScreen)
        {
            Settings settings = GetCurrentGameSettings();
            settings.FullScreen = fullScreen;
            SetGameSettings(settings);
        }

        public void SetResolution(int resIndex)
        {
            Settings settings = GetCurrentGameSettings();
            settings.VideoResolutionX = _resolutions[resIndex].width;
            settings.VideoResolutionY = _resolutions[resIndex].height;
            settings.VideoRefreshRateNumerator = _resolutions[resIndex].refreshRateRatio.numerator;
            SetGameSettings(settings);
        }


        private void InitScreenResolutions()
        {
            _resolutions = FillResolutions();
            _resolutionsDropdown.ClearOptions();

            int curResIndex = Array.FindIndex(_resolutions,
                (i) => i.width == Screen.width
                    && i.height == Screen.height
                    && i.refreshRateRatio.numerator == Screen.currentResolution.refreshRateRatio.numerator
                    && i.refreshRateRatio.denominator == Screen.currentResolution.refreshRateRatio.denominator
                );
            curResIndex = curResIndex >= 0 ? curResIndex : 0;
            List<string> options = new List<string>(Array.ConvertAll(
                _resolutions,
                (srcRes) => $"{srcRes.width}x{srcRes.height} {srcRes.refreshRateRatio.value}Hz"
            // srcRes.refreshRateRatio.value == srcRes.refreshRateRatio.numerator / srcRes.refreshRateRatio.denominator
            ));
            _resolutionsDropdown.AddOptions(options);
            _resolutionsDropdown.value = curResIndex;
            _resolutionsDropdown.RefreshShownValue();
        }

        Resolution[] FillResolutions()
        {
            if (!_hideSameResolutionsWithLowRefreshRate)
            {
                return Screen.resolutions;
            }

            SortedDictionary<Vector2Int, Resolution> resMap = new(Comparer<Vector2Int>.Create((v1, v2) => v2.x - v1.x));
            for (int i = 0; i < Screen.resolutions.Length; i++)
            {
                Resolution res = Screen.resolutions[i];
                Resolution oldRes;
                Vector2Int xy = new Vector2Int(res.width, res.height);
                if (resMap.TryGetValue(xy, out oldRes))
                {
                    if (res.refreshRateRatio.value > oldRes.refreshRateRatio.value)
                    {
                        resMap[xy] = res;
                    }
                }
                else
                {
                    resMap[xy] = res;
                }
            }

            Resolution[] resArray = new Resolution[resMap.Count];
            resMap.Values.CopyTo(resArray, 0);
            return resArray;
        }


        private void InitQualityDropdown()
        {
            _qualityDropdown.ClearOptions();
            _qualityDropdown.AddOptions(new List<string>(QualitySettings.names));
            _qualityDropdown.value = QualitySettings.GetQualityLevel();
            _qualityDropdown.RefreshShownValue();
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
    }
}
