using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum HumanJob
{
    Farmer,
    Architect,
    Doctor,
    Engineer
}

[CreateAssetMenu]
public class SomethingCoolDataHolder : ScriptableObject
{
    public string name;
    public HumanJob job;
    public Sprite Portrait;
}
