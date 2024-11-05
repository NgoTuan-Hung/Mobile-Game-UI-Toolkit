using UnityEngine;

public class ViewBase : MonoBehaviour
{
    protected GameUIManager gameUIManager;
    public GameUIManager GameUIManager { get => gameUIManager; set => gameUIManager = value; }
}