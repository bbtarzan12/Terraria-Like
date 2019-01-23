
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileChunk
{

    public BoundsInt bound;
    public TileBase[] tiles;
    public Tilemap tilemap;
    
    
    public TileChunk(BoundsInt bound, TileBase[] source, Tilemap tilemap)
    {
        this.bound = bound;
        this.tilemap = tilemap;
        
        tiles = new TileBase[source.Length];
        Array.Copy(source, tiles, source.Length);
    }

    public void Draw()
    {
        tilemap.SetTilesBlock(bound, tiles);
    }
    
    public void Erase()
    {
        Array.Clear(tiles, 0, tiles.Length);
        tilemap.SetTilesBlock(bound, tiles);
    }
}