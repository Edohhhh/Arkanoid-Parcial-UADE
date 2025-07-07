using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BrickRowConfig
{
    [Range(1, 3)] public int hitsToBreak = 1;

    [Tooltip("Columna del atlas (color visual) asociada a este nivel de golpe")]
    public int atlasColumn = 0;
}
