using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConfigMenuItem", menuName = "ScriptableObjects/ConfigMenuItem", order = 1)]
public class ConfigMenuItem : ScriptableObject
{
    public enum ConfigType {Sound, Video, Other}
    public ConfigType configType;
    public List<ConfigContentItem> configContentItems = new List<ConfigContentItem>();
}