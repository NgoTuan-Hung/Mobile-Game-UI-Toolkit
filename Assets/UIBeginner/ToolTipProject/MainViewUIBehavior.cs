using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.EnhancedTouch;

public class MainViewUIBehavior : MonoBehaviour
{
    BaseAction baseAction;

    private void Awake() {
        baseAction = new BaseAction();
    }

    private void OnEnable() {
        baseAction.Enable();
    }

    private void OnDisable() {
        baseAction.Disable();
    }
}
