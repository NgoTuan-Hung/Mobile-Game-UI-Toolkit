using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quiz
{
    /// <summary>
    /// The GameplaySounds class in the Quiz namespace manages the playback of various audio effects
    /// in response to game and UI events. It requires an AudioManager component and uses it to play
    /// the appropriate audio clips for events such as correct or incorrect answers, game won or lost,
    /// and button clicks. 
    /// </summary>
    [RequireComponent(typeof(AudioManager))]
    public class GameplaySounds : MonoBehaviour
    {
        [Tooltip("Required AudioManager component")]
        [SerializeField] AudioManager m_AudioManager;

        // Constant value to set a delay for end game sounds
        const float k_EndGameDelay = 0.5f;

        // Flag to store if the player is a winner
        bool m_IsWinner;

        // Subscribe to game events

        // Note: we could formalize the event-handling method names to $"{sender}_{event}",
        // but instead we just reuse some simple methods to playback clicks and beeps
        // for flexibility
        private void OnEnable()
        {
            if (m_AudioManager == null)
                m_AudioManager = GetComponent<AudioManager>();

            GameEvents.CorrectlyAnswered += PlayCorrectSound;
            GameEvents.IncorrectlyAnswered += PlayIncorrectSound;
            GameEvents.GameWon += SetWinner;
            GameEvents.GameLost += SetLoser;
            GameEvents.GameStarted += PlayClickSound;
            GameEvents.QuizContinued += PlayClickSound;
            GameEvents.AnswerSelected += PlayClickSound;

            UIEvents.MainMenuShown += PlayClickSound;
            UIEvents.PauseScreenShown += PlayClickSound;
            UIEvents.LevelSelectionShown += PlayClickSound;
            UIEvents.SettingsShown += PlayClickSound;
            UIEvents.EndScreenShown += PlayEndScreenSound;
            UIEvents.ScreenClosed += PlayClickSound;
            UIEvents.UrlOpened += PlayClickSound;

            LevelSelectionEvents.ButtonSelected += PlayClickSound;

            SceneEvents.LastSceneUnloaded += PlayClickSound;
            SceneEvents.SceneIndexLoaded += PlayClickSound;
            SceneEvents.UnloadSceneByPath += PlayClickSound;
            SceneEvents.LoadSceneByPath += PlayClickSound;

            DemoEvents.TextPageChanged += PlayClickSound;
        }

        // Unsubscribe to prevent errors
        private void OnDisable()
        {
            GameEvents.CorrectlyAnswered -= PlayCorrectSound;
            GameEvents.IncorrectlyAnswered -= PlayIncorrectSound;
            GameEvents.GameWon -= SetWinner;
            GameEvents.GameLost -= SetLoser;
            GameEvents.AnswerSelected -= PlayClickSound;
            GameEvents.GameStarted -= PlayClickSound;
            GameEvents.QuizContinued -= PlayClickSound;

            UIEvents.MainMenuShown -= PlayClickSound;
            UIEvents.PauseScreenShown -= PlayClickSound;
            UIEvents.LevelSelectionShown -= PlayClickSound;
            UIEvents.SettingsShown -= PlayClickSound;
            UIEvents.EndScreenShown -= PlayEndScreenSound;
            UIEvents.ScreenClosed -= PlayClickSound;
            UIEvents.UrlOpened -= PlayClickSound;

            LevelSelectionEvents.ButtonSelected -= PlayClickSound;

            SceneEvents.LastSceneUnloaded -= PlayClickSound;
            SceneEvents.SceneIndexLoaded -= PlayClickSound;
            SceneEvents.UnloadSceneByPath -= PlayClickSound;
            SceneEvents.LoadSceneByPath -= PlayClickSound;

            DemoEvents.TextPageChanged -= PlayClickSound;
        }

        private void Start()
        {
            // Verifies required fields in the Inspector
            NullRefChecker.Validate(this);

            // Verifies the required AudioClips in the Audio Settings ScriptableObject
            NullRefChecker.Validate(m_AudioManager.AudioSettings);
        }

        // Play the correct answer sound effect
        // Note: message string is a placeholder to match function signature
        private void PlayCorrectSound(string message)
        {
            m_AudioManager.PlaySFXAtPoint(m_AudioManager.AudioSettings.SuccessSound, Vector3.zero, 0f, false);
        }

        // Play the incorrect answer sound effect
        // Note: message string is a placeholder to match function signature
        private void PlayIncorrectSound(string message)
        {
            m_AudioManager.PlaySFXAtPoint(m_AudioManager.AudioSettings.FailSound, Vector3.zero, 0f, false);
        }

        // Play the game won sound effect
        private void PlayGameWonSound()
        {
            m_AudioManager.PlaySFXAtPoint(m_AudioManager.AudioSettings.GameWonSound, Vector3.zero, k_EndGameDelay, false);
        }

        // Play the game lost sound effect
        private void PlayGameLostSound()
        {
            m_AudioManager.PlaySFXAtPoint(m_AudioManager.AudioSettings.GameLostSound, Vector3.zero, k_EndGameDelay, false);
        }

        // Play the end screen sound based on whether the player has won or lost
        private void PlayEndScreenSound()
        {
            // Play the click for the Continue button
            PlayClickSound();

            // Play win/lose sound after short delay
            if (m_IsWinner)
                PlayGameWonSound();
            else
                PlayGameLostSound();
        }

        // Store the win/lose state to delay playback of the endscreen sound

        // Set the player as a loser
        private void SetLoser()
        {
            m_IsWinner = false;
        }

        // Set the player as a winner
        private void SetWinner()
        {
            m_IsWinner = true;
        }

        // Plays UI button click sound
        private void PlayClickSound()
        {
            m_AudioManager.PlaySFXAtPoint(m_AudioManager.AudioSettings.TapClickSound, Vector3.zero, 0f, false);
        }

        // Overloaded (ignore the message string parameter)

        private void PlayClickSound(List<string> strings)
        {
            PlayClickSound();
        }

        private void PlayClickSound(string str)
        {
            PlayClickSound();
        }

        private void PlayClickSound(int index)
        {
            PlayClickSound();
        }
    }
}
