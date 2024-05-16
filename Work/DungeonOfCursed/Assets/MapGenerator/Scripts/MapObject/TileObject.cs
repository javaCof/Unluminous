using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject : MonoBehaviour, IPoolObject
{
    [System.Serializable] public class TileRender
    {
        public Renderer render;
        public Vector2 tiling;
    }

    public List<TileRender> floorRenderOptions;
    public List<TileRender> wallRenderOptions;

    public void ChangeMat(Material floorMat, Material wallMat)
    {
        foreach (var tile in floorRenderOptions)
        {
            tile.render.material = floorMat;
            tile.render.material.mainTextureScale = tile.tiling;
        }
        foreach (var tile in wallRenderOptions)
        {
            tile.render.material = wallMat;
            tile.render.material.mainTextureScale = tile.tiling;
        }
    }

    public void OnPoolCreate(int id) { }
    public void OnPoolDisable() { }
    public void OnPoolEnable(Vector3 pos, Quaternion rot) { }
}
