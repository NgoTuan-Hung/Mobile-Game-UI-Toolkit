using System;
using UnityEngine.UIElements;

namespace Quiz
{
    /// <summary>
    /// Represents the settings screen (SettingsScreen.uxml), handles UI interaction and updates. The Sliders display
    /// values on their corresponding Labels with some custom behavior.
    /// </summary>
    public class SettingsScreen : UIScreen
    {
        // UI element references
        Slider m_MasterVolumeSlider;
        Slider m_SFXVolumeSlider;
        Slider m_MusicVolumeSlider;

        Label m_MasterVolumeLabel;
        Label m_SFXVolumeLabel;
        Label m_MusicVolumeLabel;
        Button m_BackButton;
        Button m_CloseButton;

        public SettingsScreen(VisualElement rootElement): base(rootElement)
        {
            SetVisualElements();
            RegisterCallbacks();
            SubscribeToEvents();

            m_IsTransparent = true;
            SettingsEvents.SettingsInitialized?.Invoke();
        }

        public override void Disable()
        {
            base.Disable();
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            // Subscribe to "*Set" events from Presenter Initialization
            SettingsEvents.MasterSliderSet += MasterVolumeSetHandler;
            SettingsEvents.SFXSliderSet += SFXVolumeSetHandler;
            SettingsEvents.MusicSliderSet += MusicVolumeSetHandler;
        }

        private void UnsubscribeFromEvents()
        {
            // Unsubscribe from "*Set" events
            SettingsEvents.MasterSliderSet -= MasterVolumeSetHandler;
            SettingsEvents.SFXSliderSet -= SFXVolumeSetHandler;
            SettingsEvents.MusicSliderSet -= MusicVolumeSetHandler;
        }

        // Find and set references to UI elements
        private void SetVisualElements()
        {
            m_MasterVolumeSlider = m_RootElement.Q<Slider>("settings__master-volume-slider");
            m_SFXVolumeSlider = m_RootElement.Q<Slider>("settings__sfx-volume-slider");
            m_MusicVolumeSlider = m_RootElement.Q<Slider>("settings__music-volume-slider");

            m_MasterVolumeLabel = m_RootElement.Q<Label>("settings__master-volume-label");
            m_SFXVolumeLabel = m_RootElement.Q<Label>("settings__sfx-volume-label");
            m_MusicVolumeLabel = m_RootElement.Q<Label>("settings__music-volume-label");

            m_BackButton = m_RootElement.Q<Button>("back-button");
            m_CloseButton = m_RootElement.Q<Button>("settings__close-button");
        }

        // Register callbacks for slider value changes; Event Registry automatically unregisters callbacks
        // on disable
        private void RegisterCallbacks()
        {
            m_EventRegistry.RegisterValueChangedCallback<float>(m_MasterVolumeSlider, MasterVolumeChangeHandler);
            m_EventRegistry.RegisterValueChangedCallback<float>(m_SFXVolumeSlider, SFXVolumeChangeHandler);
            m_EventRegistry.RegisterValueChangedCallback<float>(m_MusicVolumeSlider, MusicVolumeChangeHandler);

            m_EventRegistry.RegisterCallback<ClickEvent>(m_BackButton, evt => CloseWindow());
            m_EventRegistry.RegisterCallback<ClickEvent>(m_CloseButton, evt => CloseWindow());
        }

        // Notify the Presenter

        // Update master volume label text / notify the presenter when slider changes
        private void MasterVolumeChangeHandler(float newValue)
        {
            m_MasterVolumeLabel.text = newValue.ToString("F0");
            SettingsEvents.MasterSliderChanged?.Invoke(newValue);
        }

        // Update SFX volume label text / notify the presenter when slider changes
        private void SFXVolumeChangeHandler(float newValue)
        {
            m_SFXVolumeLabel.text = newValue.ToString("F0");
            SettingsEvents.SFXSliderChanged?.Invoke(newValue);
        }

        // Update music volume label text / notify the presenter when slider changes
        private void MusicVolumeChangeHandler(float newValue)
        {
            m_MusicVolumeLabel.text = newValue.ToString("F0");
            SettingsEvents.MusicSliderChanged?.Invoke(newValue);
        }

        // Receive notifications from the Presenter

        // Update master volume values from Presenter Initializtion
        private void MasterVolumeSetHandler(float volume)
        {
            m_MasterVolumeSlider.value = volume;
            m_MasterVolumeLabel.text = volume.ToString("F0");
        }

        // Update SFX volume values from Presenter Initializtion
        private void SFXVolumeSetHandler(float volume)
        {
            m_SFXVolumeSlider.value = volume;
            m_SFXVolumeLabel.text = volume.ToString("F0");
        }

        // Update music volume values from Presenter Initializtion
        private void MusicVolumeSetHandler(float volume)
        {
            m_MusicVolumeSlider.value = volume;
            m_MusicVolumeLabel.text = volume.ToString("F0");
        }

        private void CloseWindow()
        {
            UIEvents.ScreenClosed?.Invoke();
        }
    }
}

