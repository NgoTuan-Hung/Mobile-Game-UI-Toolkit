using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Quiz
{
    /// <summary>
    /// A SequenceManager controls the overall flow of the application using a state machine.
    /// 
    /// Use this class to define how each State will transition to the next. Each state can
    /// transition to the next state when receiving an event or reaching a specific condition.
    ///
    /// Note: this class currently is only used for demonstration/diagnostic purposes. You can use
    ///       the start and end of each state to instantiate GameObjects/play effects. Another simple
    ///       state machine for UI screens (UIManager) actually drives most of the quiz gameplay.
    /// 
    /// </summary>

    public class SequenceManager : MonoBehaviour
    {
        // Inspector fields
        [Header("Preload (Splash Screen)")]
        [Tooltip("Prefab assets that load first. These can include level management Prefabs or textures, sounds, etc.")]
        [SerializeField] GameObject[] m_PreloadedAssets;

        [Tooltip("Time in seconds to show Splash Screen")]
        [SerializeField] float m_LoadScreenTime = 5f;

        [Space(10)]
        [Tooltip("Debug state changes in the console")]
        [SerializeField] bool m_Debug;

        StateMachine m_StateMachine = new StateMachine();

        // Define all States here
        IState m_SplashScreenState;     // Startup and load assets, show a splash screen
        IState m_StartScreenState;      // Empty screen with a start button

        IState m_MainMenuState;         // Show the main menu screens
        IState m_LevelSelectionState;   // Show a UI Screen to choose a game mode/level
        IState m_MenuSettingsState;     // Show the Settings Screen while in the Main Menu
        IState m_GamePlayState;         // Play the game
        IState m_GameSettingsState;     // Go to the Settings Screen during gameplay
        IState m_PauseState;            // Pause the game during gameplay

        IState m_GameWinState;          // Show the win screen
        IState m_GameLoseState;         // Show the lose screen

        // Access to the StateMachine's CurrentState
        public IState CurrentState => m_StateMachine.CurrentState;

        #region MonoBehaviour event messages
        private void Start()
        {
            // Set this MonoBehaviour to control the coroutines - unused in this demo
            Coroutines.Initialize(this);

            // Checks for required fields in the Inspector
            NullRefChecker.Validate(this);

            // Instantiates any assets needed before gameplay
            InstantiatePreloadedAssets();

            // Sets up States and transitions, runs initial State
            Initialize();
        }

        // Subscribe to event channels
        private void OnEnable()
        {
            SceneEvents.ExitApplication += SceneEvents_ExitApplication;
        }

        // Unsubscribe from event channels to prevent errors
        private void OnDisable()
        {
            SceneEvents.ExitApplication -= SceneEvents_ExitApplication;
        }
        #endregion

        #region Methods

        public void Initialize()
        {
            // Define the Game States
            SetStates();
            AddLinks();

            // Run first state/loading screen
            m_StateMachine.Run(m_SplashScreenState);
            UIEvents.SplashScreenShown?.Invoke();
        }

        // Define the state machine's states
        private void SetStates()
        {
            // Create States for the game. Pass in an Action to execute or null to do nothing

            // Optional names added for debugging
            // Executes GameEvents.LoadProgressUpdated every frame and GameEvents.PreloadCompleted on exit
            m_SplashScreenState = new DelayState(m_LoadScreenTime, SceneEvents.LoadProgressUpdated,
                SceneEvents.PreloadCompleted, "LoadScreenState");

            m_StartScreenState = new State(null, "StartScreenState", m_Debug);
            m_MainMenuState = new State(null, "MainMenuState", m_Debug);
            m_LevelSelectionState = new State(null, "LevelSelectionState", m_Debug);
            m_MenuSettingsState = new State(null, "SettingsState", m_Debug);
            m_GamePlayState = new State(null, "GamePlayState", m_Debug);
            m_PauseState = new State(null, "PauseState", m_Debug);
            m_GameSettingsState = new State(null, "GameSettingsState", m_Debug);
            m_GameWinState = new State(null, "GameWinState", m_Debug);
            m_GameLoseState = new State(null, "GameLoseState", m_Debug);
        }


        // Define links between the states
        private void AddLinks()
        {

            // Transition automatically to the StartScreen once the loading time completes
            m_SplashScreenState.AddLink(new Link(m_StartScreenState));

            // EventLinks listen for the UI/game event messages to activate the transition to the next state

            // This implementation uses a wrapper around the event to make easier to register/unregister the EventLinks
            ActionWrapper mainMenuShownWrapper = new ActionWrapper
            {
                Subscribe = handler => UIEvents.MainMenuShown += handler,
                Unsubscribe = handler => UIEvents.MainMenuShown -= handler
            };

            ActionWrapper levelSelectionShownWrapper = new ActionWrapper
            {
                Subscribe = handler => UIEvents.LevelSelectionShown += handler,
                Unsubscribe = handler => UIEvents.LevelSelectionShown -= handler
            };

            ActionWrapper settingsShownWrapper = new ActionWrapper
            {
                Subscribe = handler => UIEvents.SettingsShown += handler,
                Unsubscribe = handler => UIEvents.SettingsShown -= handler
            };

            ActionWrapper screenClosedWrapper = new ActionWrapper
            {
                Subscribe = handler => UIEvents.ScreenClosed += handler,
                Unsubscribe = handler => UIEvents.ScreenClosed -= handler
            };

            ActionWrapper gameStartedWrapper = new ActionWrapper
            {
                Subscribe = handler => GameEvents.GameStarted += handler,
                Unsubscribe = handler => GameEvents.GameStarted -= handler
            };
  
            ActionWrapper pauseScreenShownWrapper = new ActionWrapper
            {
                Subscribe = handler => UIEvents.PauseScreenShown += handler,
                Unsubscribe = handler => UIEvents.PauseScreenShown -= handler
            };
  
            ActionWrapper gameWonWrapper = new ActionWrapper
            {
                Subscribe = handler => GameEvents.GameWon += handler,
                Unsubscribe = handler => GameEvents.GameWon -= handler
            };

            ActionWrapper gameLostWrapper = new ActionWrapper
            {
                Subscribe = handler => GameEvents.GameLost += handler,
                Unsubscribe = handler => GameEvents.GameLost -= handler
            };

            // Once you have wrappers defined around the events, set up the EventLinks
  
            m_StartScreenState.AddLink(new EventLink(mainMenuShownWrapper, m_MainMenuState));

            m_MainMenuState.AddLink(new EventLink(levelSelectionShownWrapper, m_LevelSelectionState));
            m_MainMenuState.AddLink(new EventLink(settingsShownWrapper, m_MenuSettingsState));

            m_MenuSettingsState.AddLink(new EventLink(screenClosedWrapper, m_MainMenuState));

            m_LevelSelectionState.AddLink(new EventLink(screenClosedWrapper, m_MainMenuState));
            m_LevelSelectionState.AddLink(new EventLink(gameStartedWrapper, m_GamePlayState));

            m_GamePlayState.AddLink(new EventLink(pauseScreenShownWrapper, m_PauseState));
            m_GamePlayState.AddLink(new EventLink(settingsShownWrapper, m_GameSettingsState));
            m_GamePlayState.AddLink(new EventLink(gameWonWrapper, m_GameWinState));
            m_GamePlayState.AddLink(new EventLink(gameLostWrapper, m_GameLoseState));

            m_GameSettingsState.AddLink(new EventLink(screenClosedWrapper, m_GamePlayState));

            m_PauseState.AddLink(new EventLink(screenClosedWrapper, m_GamePlayState));
            m_PauseState.AddLink(new EventLink(mainMenuShownWrapper, m_MainMenuState));

            m_GameWinState.AddLink(new EventLink(mainMenuShownWrapper, m_MainMenuState));

            m_GameLoseState.AddLink(new EventLink(mainMenuShownWrapper, m_MainMenuState));
            m_GameLoseState.AddLink(new EventLink(gameStartedWrapper, m_GamePlayState));
        }

        // Use this to preload any assets. The QuizU sample only loads a few prefabs, but this is an
        // opportunity to load any textures, models, etc. in advance to avoid loading during gameplay 
        private void InstantiatePreloadedAssets()
        {
            foreach (var asset in m_PreloadedAssets)
            {
                if (asset != null)
                    Instantiate(asset);
            }
        }
        #endregion

        // Event-handling methods
        private void SceneEvents_ExitApplication()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
