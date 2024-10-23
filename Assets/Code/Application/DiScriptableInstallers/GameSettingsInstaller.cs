using UnityEngine;
using UnityEngine.Audio;
using VContainer;

namespace ComixArea.Configuration
{
    [CreateAssetMenu(fileName = "Settings Installer", menuName = "Comix Area/DI Installers/Settings")]
    public class GameSettingsInstaller : AScriptableInstaller
    {
        [SerializeField]
        private AudioMixer _audioMixer;
        [SerializeField]
        private AudioMixerGroup _masterAudioMixerGroup;
        [SerializeField]
        private AudioMixerGroup _musicAudioMixerGroup;
        [SerializeField]
        private AudioMixerGroup _sfxAudioMixerGroup;
        [SerializeField]
        private string _masterVolumeAudioMixerParamName = "masterVolume";
        [SerializeField]
        private string _musicVolumeAudioMixerParamName = "musicVolume";
        [SerializeField]
        private string _sfxVolumeAudioMixerParamName = "soundsVolume";
        [SerializeField]
        private float _minVolumeDb = -80;
        [SerializeField]
        private float _maxVolumeDb = 20;
        [SerializeField]
        private float _initialVolumeDb = 0;

        public override void Install(IContainerBuilder builder)
        {
            builder.Register<SettingsManager>(Lifetime.Scoped)
                .WithParameter(SettingsManager.ParamNameAudioMixer, _audioMixer)
                .WithParameter(SettingsManager.ParamNameMasterAudioMixerGroup, _masterAudioMixerGroup)
                .WithParameter(SettingsManager.ParamNameMusicAudioMixerGroup, _musicAudioMixerGroup)
                .WithParameter(SettingsManager.ParamNameSoundsAudioMixerGroup, _sfxAudioMixerGroup)
                .WithParameter(SettingsManager.ParamNameMasterVolumeAudioMixerParamName, _masterVolumeAudioMixerParamName)
                .WithParameter(SettingsManager.ParamNameMusicVolumeAudioMixerParamName, _musicVolumeAudioMixerParamName)
                .WithParameter(SettingsManager.ParamNameSfxVolumeAudioMixerParamName, _sfxVolumeAudioMixerParamName)
                .WithParameter(SettingsManager.ParamNameMinVolumeDb, _minVolumeDb)
                .WithParameter(SettingsManager.ParamNameMaxVolumeDb, _maxVolumeDb)
                .WithParameter(SettingsManager.ParamNameInitialVolumeDb, _initialVolumeDb)
                .As<ISettingsManager>();
        }
    }
}
