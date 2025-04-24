using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class SetAtlasTile : MonoBehaviour
{
    [Header("Configuración del Atlas")]
    public int atlasColumn = 0;
    public int atlasRow = 0;
    public int columns = 3;
    public int rows = 2;

    private void OnValidate()
    {
        
        ApplyTile();
    }

    public void ApplyTile()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();

        if (renderer.sharedMaterial == null) return;

        
        Material mat = Application.isPlaying ? renderer.material : renderer.sharedMaterial;

        Vector2 tiling = new Vector2(1f / columns, 1f / rows);
        Vector2 offset = new Vector2(atlasColumn * tiling.x, 1f - tiling.y - atlasRow * tiling.y);

        mat.mainTextureScale = tiling;
        mat.mainTextureOffset = offset;
    }
}

