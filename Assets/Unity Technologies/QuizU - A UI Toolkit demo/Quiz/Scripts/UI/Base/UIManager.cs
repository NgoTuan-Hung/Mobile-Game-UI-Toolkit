using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Quiz
{
    /// <summary>
    /// The UI Manager manages the UI screens (View base class) using GameEvents paired
    /// to each View screen. A stack maintains a history of previously shown screens, so
    /// the UI Manager can "go back" until it reaches the default UI screen, the home screen.
    ///
    /// To add a new UIScreen under the UIManager's management:
    ///    -Define a new UIScreen field
    ///    -Create a new instance of that screen in Initialize (e.g. new SplashScreen(root.Q<VisualElement>("splash__container"));
    ///    -Register the UIScreen in the RegisterScreens method
    ///    -Subscribe/unsubscribe from the appropriate UIEvent to show the screen
    ///
    /// Alternatively, use Reflection to add the UIScreen to the RegisterScreens method
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Tooltip("Required UI Document")]
        [SerializeField] UIDocument m_Document;

        // Load screen with a progress bar, displays after application launch
        UIScreen m_SplashScreen;

        // First screen that loads after the splash screen, shown only once after loading
        UIScreen m_StartScreen;

        // Primary modal screen (e.g. main menu)
        UIScreen m_HomeScreen;

        // Tabbed menu screen that shows settings for audio sliders
        UIScreen m_SettingsScreen;

        // Screen to choose a quiz to play
        UIScreen m_LevelSelectionScreen;

        // The main gameplay screen that shows a single question
        UIScreen m_GameScreen;

        // In-game screen that allows the user to quit or continue
        UIScreen m_PauseScreen;

        // The final screen showing quiz results
        UIScreen m_EndScreen;

        // The currently active UIScreen
        UIScreen m_CurrentScreen;

        // A stack of previously displayed UIScreens
        Stack<UIScreen> m_History = new Stack<UIScreen>();

        // A list of all Views to show/hide
        List<UIScreen> m_Screens = new List<UIScreen>();



        public UIScreen CurrentScreen => m_CurrentScreen;
        public UIDocument Document => m_Document;

        // Register event listeners to game events
        private void OnEnable()
        {
            SubscribeToEvents();

            // Because non-MonoBehaviours can't run coroutines, the Coroutines helper utility allows us to
            // designate a MonoBehaviour to manage starting/stopping coroutines
            Coroutines.Initialize(this);

            Initialize();
        }

        // Unregister the listeners to prevent errors
        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            // Wait for the SplashScreen to finish loading then load the StartScreen
            SceneEvents.PreloadCompleted += SceneEvents_PreloadCompleted;

            // Pair GameEvents with methods to Show each screen
            UIEvents.SplashScreenShown += UIEvents_SplashScreenShown;
            UIEvents.MainMenuShown += UIEvents_MainMenuShown;
            UIEvents.SettingsShown += UIEvents_SettingsShown;
            UIEvents.LevelSelectionShown += UIEvents_LevelSelectionShown;
            UIEvents.GameScreenShown += UIEvents_GameScreenShown;
            UIEvents.PauseScreenShown += UIEvents_PauseScreenShown;
            UIEvents.EndScreenShown += UIEvents_EndScreenShown;
            UIEvents.ScreenClosed += UIEvents_ScreenClosed;
            UIEvents.UrlOpened += UIEvents_UrlOpened;
        }

        private void UnsubscribeFromEvents()
        {
            SceneEvents.PreloadCompleted -= SceneEvents_PreloadCompleted;

            UIEvents.SplashScreenShown -= UIEvents_SplashScreenShown;
            UIEvents.MainMenuShown -= UIEvents_MainMenuShown;
            UIEvents.SettingsShown -= UIEvents_SettingsShown;
            UIEvents.LevelSelectionShown -= UIEvents_LevelSelectionShown;
            UIEvents.GameScreenShown -= UIEvents_GameScreenShown;
            UIEvents.PauseScreenShown -= UIEvents_PauseScreenShown;
            UIEvents.EndScreenShown -= UIEvents_EndScreenShown;
            UIEvents.ScreenClosed -= UIEvents_ScreenClosed;
            UIEvents.UrlOpened -= UIEvents_UrlOpened;
        }

        // Event-handling methods

        // Show the SplashScreen and don't keep in history
        private void UIEvents_SplashScreenShown()
        {
            Show(m_SplashScreen, false);
        }

        // Show the StartScreen but don't keep it in the history
        private void SceneEvents_PreloadCompleted()
        {
            Show(m_StartScreen, false);
        }

        // Clear the History and make the HomeScreen (MainMenu) the only View
        public void UIEvents_MainMenuShown()
        {
  
            m_CurrentScreen = m_HomeScreen;

            HideScreens();
            m_History.Push(m_HomeScreen);
            m_HomeScreen.Show();
        }

        private void UIEvents_SettingsShown()
        {
            Show(m_SettingsScreen);
        }

        private void UIEvents_LevelSelectionShown()
        {
            Show(m_LevelSelectionScreen);
        }

        private void UIEvents_GameScreenShown()
        {
            Show(m_GameScreen);
        }
        private void UIEvents_PauseScreenShown()
        {
            Show(m_PauseScreen);
        }

        private void UIEvents_EndScreenShown()
        {
            Show(m_EndScreen);
        }

        // Remove the top UI screen from the stack and make that active (i.e., go back one screen)
        public void UIEvents_ScreenClosed()
        {
            if (m_History.Count != 0)
            {
                Show(m_History.Pop(), false);
            }
        }

        private void UIEvents_UrlOpened(string link)
        {
            Application.OpenURL(link);
        }

        // Methods

        // Clears history and hides all Views except the Start Screen
        private void Initialize()
        {
            NullRefChecker.Validate(this);

            VisualElement root = m_Document.rootVisualElement;

            m_SplashScreen = new SplashScreen(root.Q<VisualElement>("splash__container"));
            m_StartScreen = new StartScreen(root.Q<VisualElement>("start__container"));
            m_HomeScreen = new MainMenuScreen(root.Q<VisualElement>("menu__container"));
            m_SettingsScreen = new SettingsScreen(root.Q<VisualElement>("settings__container"));
            m_LevelSelectionScreen = new LevelSelectionScreen(root.Q<VisualElement>("select__container"));
            m_GameScreen = new GameScreen(root.Q<VisualElement>("question-screen__parent"));
            m_PauseScreen = new PauseScreen(root.Q<VisualElement>("pause__container"));
            m_EndScreen = new EndScreen(root.Q<VisualElement>("end-screen__container"));

            // Notify the GameController the UIScreen for LevelSelection has been setup
            LevelSelectionEvents.Initialized?.Invoke(m_LevelSelectionScreen as LevelSelectionScreen);

            RegisterScreens();
            HideScreens();
        }

        // Store each UIScreen into a master list so we can hide all of them easily.
        private void RegisterScreens()
        {
            m_Screens = new List<UIScreen>
            {
                m_SplashScreen,
                m_StartScreen,
                m_HomeScreen,
                m_SettingsScreen,
                m_LevelSelectionScreen,
                m_GameScreen,
                m_PauseScreen,
                m_EndScreen
            };
        }

        // Clear history and hide all Views
        private void HideScreens()
        {
            m_History.Clear();

            foreach (UIScreen screen in m_Screens)
            {
                screen.Hide();
            }
        }

        // Finds the first registered UI View of the specified type T
        public T GetScreen<T>() where T : UIScreen
        {
            foreach (var screen in m_Screens)
            {
                if (screen is T typeOfScreen)
                {
                    return typeOfScreen;
                }
            }
            return null;
        }

        // Shows a View of a specific type T, with the option to add it
        // to the history stack
        public void Show<T>(bool keepInHistory = true) where T : UIScreen
        {
            foreach (var screen in m_Screens)
            {
                if (screen is T)
                {
                    Show(screen, keepInHistory);
                    break;
                }
            }
        }

        // 
        public void Show(UIScreen screen, bool keepInHistory = true)
        {
            if (screen == null)
                return;

            if (m_CurrentScreen != null)
            {
                if (!screen.IsTransparent)
                    m_CurrentScreen.Hide();

                if (keepInHistory)
                {
                    m_History.Push(m_CurrentScreen);
                }
            }

            screen.Show();
            m_CurrentScreen = screen;
        }

        // Shows a UIScreen with the keepInHistory always enabled
        public void Show(UIScreen screen)
        {
            Show(screen, true);
        }
    }
}

