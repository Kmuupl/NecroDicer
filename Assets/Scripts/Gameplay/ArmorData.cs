using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ArmorConditionData
{
    public int requiredValue;
    public bool mustBeGreater;
}

[CreateAssetMenu(menuName = "NecroDicer/Armor")]
public class ArmorData : ScriptableObject
{
    public List<ArmorConditionData> conditions;
    public float[] damagePercents;
}