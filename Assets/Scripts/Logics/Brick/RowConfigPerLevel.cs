using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LevelConfig/RowConfigPerLevel")]
public class RowConfigPerLevel : ScriptableObject
{
    public List<BrickRowConfig> rowConfigs;
}
