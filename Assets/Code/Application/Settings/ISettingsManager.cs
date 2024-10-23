﻿
namespace ComixArea.Configuration
{
    public interface ISettingsManager
    {
        public void ActivateSettings(Settings settings);

        public Settings LoadSettings();

        public void SaveSettings(Settings settings);

        public Settings GetCurrentGameSettings();

        public Settings AssembleCurrentSettings();
    }
}
