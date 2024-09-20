using System;
using UnityEngine;

namespace Quiz
{
    /// <summary>
    ///  Controller/presenter to manage the sound volume settings and UI. This
    ///  sends messages between the user-interface (the View) and the AudioSettings
    ///  ScriptableObject data (the Model).
    /// </summary>
    public class SettingsPresenter : MonoBehaviour
    {
        // Though we can use events to communicate with the Model and View, we can also
        // directly reference them for convenience
        AudioSettingsSO m_AudioSettingsSO;

        // Event subscriptions
        private void OnEnable()
        {
            // Listen for events from the View/UI
            SettingsEvents.MasterSliderChanged += SettingsEvents_MasterSliderChanged;
            SettingsEvents.SFXSliderChanged += SettingsEvents_SFXSliderChanged;
            SettingsEvents.MusicSliderChanged += SettingsEvents_MusicSliderChanged;

            // Listen for events from the Model
            SettingsEvents.ModelMasterVolumeChanged += SettingsEvents_ModelMasterVolumeChanged;
            SettingsEvents.ModelSFXVolumeChanged += SettingsEvents_ModelSFXVolumeChanged;
            SettingsEvents.ModelMusicVolumeChanged += SettingsEvents_ModelMusicVolumeChanged;
        }

        // Event unsubscriptions
        private void OnDisable()
        {
            SettingsEvents.MasterSliderChanged -= SettingsEvents_MasterSliderChanged;
            SettingsEvents.SFXSliderChanged -= SettingsEvents_SFXSliderChanged;
            SettingsEvents.MusicSliderChanged -= SettingsEvents_MusicSliderChanged;

            SettingsEvents.ModelMasterVolumeChanged -= SettingsEvents_ModelMasterVolumeChanged;
            SettingsEvents.ModelSFXVolumeChanged -= SettingsEvents_ModelSFXVolumeChanged;
            SettingsEvents.ModelMusicVolumeChanged -= SettingsEvents_ModelMusicVolumeChanged;
        }

        private void Start()
        {
            Initialize();
        }

        // Set defaults
        private void Initialize()
        {
            m_AudioSettingsSO = Resources.Load<AudioSettingsSO>("Audio/AudioSettings_Data");

            // Update sliders with the default values from the AudioSettingsSO
            float masterVolume = m_AudioSettingsSO.MasterVolume * 100f;
            float sfxVolume = m_AudioSettingsSO.SoundEffectsVolume * 100f;
            float musicVolume = m_AudioSettingsSO.MusicVolume * 100f;

            // Notify the View of default values from the Model
            SettingsEvents.MasterSliderSet?.Invoke(masterVolume);
            SettingsEvents.SFXSliderSet?.Invoke(sfxVolume);
            SettingsEvents.MusicSliderSet?.Invoke(musicVolume);
        }

        // Event-handling methods

        // View event handlers
        public void SettingsEvents_MasterSliderChanged(float sliderValue)
        {
            float volume = sliderValue / 100f;
            SettingsEvents.MasterVolumeChanged?.Invoke(volume);
        }

        public void SettingsEvents_SFXSliderChanged(float sliderValue)
        {
            float volume = sliderValue / 100f;
            SettingsEvents.SFXVolumeChanged?.Invoke(volume);
        }

        public void SettingsEvents_MusicSliderChanged(float sliderValue)
        {
            float volume = sliderValue / 100f;
            SettingsEvents.MusicVolumeChanged?.Invoke(volume);
        }

        // Model event handlers (response if Model data externally modified,
        // e.g. loading preferences from disk)
        private void SettingsEvents_ModelMasterVolumeChanged(float volume)
        {
            // Process the master volume change from the Model
            SettingsEvents.MasterSliderSet?.Invoke(volume);
        }

        private void SettingsEvents_ModelSFXVolumeChanged(float volume)
        {
            // Process the SFX volume change from the Model
            SettingsEvents.SFXSliderSet?.Invoke(volume);
        }

        private void SettingsEvents_ModelMusicVolumeChanged(float volume)
        {
            // Process the music volume change from the Model
            SettingsEvents.MusicSliderSet?.Invoke(volume);
        }
    }
}
