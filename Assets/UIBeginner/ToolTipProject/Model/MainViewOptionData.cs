using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MainViewOptionData", menuName = "ScriptableObjects/MainViewOptionData", order = 1)]
public class MainViewOptionData : ScriptableObject
{
    public Texture2D icon;
    public String functionName;
}