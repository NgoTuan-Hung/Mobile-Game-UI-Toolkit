using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Quiz
{
    /// <summary>
    /// The LifeBarDisplay class manages a segmented UI health bar. Each segment is separate child VisualElement
    /// (some icon that represents one unit of health). This example allows for a maximum of
    /// k_MaxSegments bar segments. Use a naming convention (e.g. life-bar_life-1, life-bar_life-2, etc. for
    /// each child VisualElement).
    ///
    /// Game events initialize and update the health bar. The LifeBar hides and shows segments through USS
    /// class manipulations and display style changes.
    /// </summary>

    public class LifeBarDisplay : IDisposable
    {
        const int k_MaxSegments = 5;

        const string k_EnabledClassName = "life-bar--enabled"; // USS Class for one bar segment
        const string k_DisableClassName = "life-bar--disabled"; // USS Class for one bar segment
        const string k_SegmentBaseName = "life-bar__life-"; // Child naming convention: 


        VisualElement m_Root;
        VisualElement m_LifeBarContainer; // Parent container
        List<VisualElement> m_Segments; // Child elements of the m_LifeBarContainer
        TooltipController m_TooltipController;

        // Constructor
        public LifeBarDisplay(VisualElement parentElement)
        {
            Initialize(parentElement);

            // Event subscriptions
            GameEvents.LivesSetup += GameEvents_LivesSetup;
            GameEvents.LivesUpdated += GameEvents_LivesUpdated;
        }


        // Event-handling methods

        private void GameEvents_LivesSetup(int maxLives)
        {
            SetupSegments(maxLives);
        }

        private void GameEvents_LivesUpdated(int livesUpdated)
        {
            UpdateSegments(livesUpdated);
        }

        // Methods
        public void Initialize(VisualElement parentElement)
        {
            m_Root = parentElement;
            m_LifeBarContainer = m_Root.Q<VisualElement>("life-bar__container");

            m_TooltipController = new TooltipController(m_Root);
            m_TooltipController.TooltipView.Offset = new Vector2(-50, -50);

        }

        public void AssignTooltip(string lifeBarTooltip)
        {
            m_TooltipController.AssignTooltipToElement(m_LifeBarContainer, lifeBarTooltip);

        }

        // Use DisplayStyle to turn off any segments beyond the initialSegments
        // Remove any leftover opacity settings

        private void SetupSegments(int initialSegments)
        {
            m_Segments = new List<VisualElement>();

            for (int i = 1; i <= k_MaxSegments; i++)
            {
                VisualElement segment = m_LifeBarContainer.Q<VisualElement>(k_SegmentBaseName + i.ToString());

                if (segment != null)
                {
                    m_Segments.Add(segment);

                    // Disable unused elements to let the other segments expand
                    if (i > initialSegments)
                    {
                        segment.style.display = DisplayStyle.None;
                    }
                    else
                    {
                        segment.style.display = DisplayStyle.Flex;
                        segment.RemoveFromClassList(k_DisableClassName);
                        segment.AddToClassList(k_EnabledClassName);
                    }
                }
            }
        }

        // Use the k_DisableClassName to disable hidden segments
        private void UpdateSegments(int remainingSegments)
        {

            // Check if m_Segments contains enough elements
            if (m_Segments.Count < remainingSegments)
            {
                Debug.LogWarning($"[LifeBar]: " +
                    $"Expected {remainingSegments}, but found {m_Segments.Count}.");
                return;
            }

            for (int i = remainingSegments; i < m_Segments.Count; i++)
            {
                // Check if m_Segments[i] is a valid element
                if (m_Segments[i] == null)
                {
                    break;
                }

                m_Segments[i].AddToClassList(k_DisableClassName);

            }
        }

        public void Dispose()
        {
            GameEvents.LivesSetup -= GameEvents_LivesSetup;
            GameEvents.LivesUpdated -= GameEvents_LivesUpdated;
        }
    }
}
