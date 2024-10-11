using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConfigData", menuName = "ScriptableObjects/ConfigData")]
public class ConfigData : ScriptableObject
{
    public List<ConfigMenuItem> configMenuItems = new List<ConfigMenuItem>();
}