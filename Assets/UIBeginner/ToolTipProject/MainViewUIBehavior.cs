using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainViewUIBehavior : MonoBehaviour
{
    BaseAction baseAction;

    private void Awake() {
        baseAction = new BaseAction();
    }

    private void Update() 
    {
        if (CheckTouchIsReleasedThisFrame()) Debug.Log("Release");
    }

    public bool CheckTouchIsReleasedThisFrame()
    {
        return baseAction.BaseMap.Touch.WasReleasedThisFrame();
    }

    private void OnEnable() {
        baseAction.Enable();
    }

    private void OnDisable() {
        baseAction.Disable();
    }
}
