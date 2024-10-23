using System;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using VContainer;

namespace ComixArea.Configuration
{
    public class SettingsManager : ISettingsManager
    {
        const string JSON_EXT = ".json";

        public const string ParamNameAudioMixer = "audioMixer";
        public const string ParamNameMasterAudioMixerGroup = "masterAudioMixerGroup";
        public const string ParamNameMusicAudioMixerGroup = "musicAudioMixerGroup";
        public const string ParamNameSoundsAudioMixerGroup = "sfxAudioMixerGroup";
        public const string ParamNameMasterVolumeAudioMixerParamName = "masterVolumeAudioMixerParamName";
        public const string ParamNameMusicVolumeAudioMixerParamName = "musicVolumeAudioMixerParamName";
        public const string ParamNameSfxVolumeAudioMixerParamName = "sfxVolumeAudioMixerParamName";
        public const string ParamNameMinVolumeDb = "minVolumeDb";
        public const string ParamNameMaxVolumeDb = "maxVolumeDb";
        public const string ParamNameInitialVolumeDb = "initialVolumeDb";

        private AudioMixer _audioMixer;
        private AudioMixerGroup _masterAudioMixerGroup;
        private AudioMixerGroup _musicAudioMixerGroup;
        private AudioMixerGroup _sfxAudioMixerGroup;
        private string _masterVolumeAudioMixerParamName = "masterVolume";
        private string _musicVolumeAudioMixerParamName = "musicVolume";
        private string _sfxVolumeAudioMixerParamName = "soundsVolume";
        private float _minVolumeDb = -80f;
        private float _maxVolumeDb = 20f;
        private float _initialVolumeDb = 0f;

        [Inject]
        public void Init(
            AudioMixer audioMixer,
            AudioMixerGroup masterAudioMixerGroup,
            AudioMixerGroup musicAudioMixerGroup,
            AudioMixerGroup sfxAudioMixerGroup,
            string masterVolumeAudioMixerParamName = "masterVolume",
            string musicVolumeAudioMixerParamName = "musicVolume",
            string sfxVolumeAudioMixerParamName = "soundsVolume",
            float minVolumeDb = -80f,
            float maxVolumeDb = 20f,
            float initialVolumeDb = 0f
        )
        {
            _audioMixer = audioMixer;
            _masterAudioMixerGroup = masterAudioMixerGroup;
            _musicAudioMixerGroup = musicAudioMixerGroup;
            _sfxAudioMixerGroup = sfxAudioMixerGroup;
            _masterVolumeAudioMixerParamName = masterVolumeAudioMixerParamName;
            _musicVolumeAudioMixerParamName = musicVolumeAudioMixerParamName;
            _sfxVolumeAudioMixerParamName = sfxVolumeAudioMixerParamName;
            _minVolumeDb = minVolumeDb;
            _maxVolumeDb = maxVolumeDb;
            _initialVolumeDb = initialVolumeDb;
        }

        public void ActivateSettings(Settings settings)
        {
            Screen.SetResolution(
                settings.VideoResolutionX,
                settings.VideoResolutionY,
                settings.FullScreen ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed,
                new RefreshRate()
                {
                    numerator = settings.VideoRefreshRateNumerator,
                    denominator = settings.VideoRefreshRateDenominator
                }
            );
            QualitySettings.SetQualityLevel(settings.GraphicsQuality);
            // Assumimng nobody invoke this method in Awake
            _masterAudioMixerGroup.audioMixer.SetFloat(
                _masterVolumeAudioMixerParamName,
                Convert01ToDb(settings.MasterVolume)
            );
            _musicAudioMixerGroup.audioMixer.SetFloat(
                _musicVolumeAudioMixerParamName,
                Convert01ToDb(settings.MusicVolume)
            );
            _sfxAudioMixerGroup.audioMixer.SetFloat(
                _sfxVolumeAudioMixerParamName,
                Convert01ToDb(settings.SfxVolume)
            );
        }

        public Settings LoadSettings()
        {
            var saveLocation = GetSettingsFilePath();

            if (!File.Exists(saveLocation))
                return null;

            var settingsString = File.ReadAllText(saveLocation);
            return JsonUtility.FromJson<Settings>(settingsString);
        }

        public void SaveSettings(Settings settings)
        {
            var settingsString = JsonUtility.ToJson(settings, true);
            File.WriteAllText(GetSettingsFilePath(), settingsString);
        }

        public Settings GetCurrentGameSettings()
        {
            var currentSettings = LoadSettings();
            if (currentSettings == null)
            {
                currentSettings = AssembleCurrentSettings();
            }
            return currentSettings;
        }

        public Settings AssembleCurrentSettings()
        {
            var settings = new Settings();
            Resolution resolution = Screen.currentResolution;
            settings.VideoResolutionX = resolution.width;
            settings.VideoResolutionY = resolution.height;
            settings.VideoRefreshRateNumerator = resolution.refreshRateRatio.numerator;
            settings.VideoRefreshRateDenominator = resolution.refreshRateRatio.denominator;
            settings.FullScreen = Screen.fullScreen;
            settings.GraphicsQuality = QualitySettings.GetQualityLevel();
            //Just for simplicity
            _masterAudioMixerGroup.audioMixer.GetFloat(_masterVolumeAudioMixerParamName, out settings.MasterVolume);
            _sfxAudioMixerGroup.audioMixer.GetFloat(_sfxVolumeAudioMixerParamName, out settings.SfxVolume);
            _musicAudioMixerGroup.audioMixer.GetFloat(_musicVolumeAudioMixerParamName, out settings.MusicVolume);
            return settings;
        }

        private static string GetPersistentDataLocation()
        {
            string gameDataLocation = Application.persistentDataPath + Path.DirectorySeparatorChar + "game_data";
            if (!Directory.Exists(gameDataLocation))
            {
                Directory.CreateDirectory(gameDataLocation);
            }
            return gameDataLocation;
        }

        private static string GetSettingsFilePath()
        {
            return $"{GetPersistentDataLocation()}{Path.DirectorySeparatorChar}settings{JSON_EXT}";
        }

        private float Convert01ToDb(float val01)
        {
            return _minVolumeDb + (_maxVolumeDb - _minVolumeDb) * val01;
        }
    }
}
