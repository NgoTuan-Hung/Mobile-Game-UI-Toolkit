using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quiz
{
    /// <summary>
    /// Public static delegates to manage level selection (note these are "events" in the conceptual sense
    /// and not the strict C# sense).
    /// </summary>
    public static class LevelSelectionEvents
    {
        // Send the LevelSelectionScreen to the 
        public static Action<LevelSelectionScreen> Initialized;

        // Send the chosen button's index to the LevelSelectionPresenter
        public static Action<int> ButtonSelected;

        // Show the selected Quiz ScriptableObject data on the LevelSelectionScreen
        public static Action<QuizSO> QuizDataLoaded;

    }
}
