using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PowerUpDropEntry
{
    public PowerUpEffectSO effect;
    [Range(0f, 1f)] public float weight = 0.33f;
}