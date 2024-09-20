using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Quiz
{
    /// <summary>
    /// Public static delegates to manage gameplay specifically for the Demo scenes (note these are "events"
    /// in the conceptual sense and not the strict C# sense). Invoked by external objects.
    /// </summary>
    public static class DemoEvents
    {
        // Notify other components that Presenter and Screen are ready
        public static Action SetupCompleted;

        // Notify the Setup component to unload the scene
        public static Action BackButtonClicked;

        // Moused over the Button in the DemoSelectionScreen
        public static Action<int> ButtonHighlighted;

        // Additively load the Demo scene with given index
        public static Action<int> SceneIndexLoaded;

        // Unload the previous scene
        public static Action LastSceneUnloaded;

        public static Action TextPageChanged;

    }
}
