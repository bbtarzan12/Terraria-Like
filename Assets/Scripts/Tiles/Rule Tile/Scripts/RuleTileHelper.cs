
using UnityEngine;
using UnityEngine.Tilemaps;

public static class RuleTileHelper
{

    static StringTilebaseDictionary tileDictionary;
    
    public static TileBase GetTile(string name)
    {
        if (tileDictionary == null)
        {
            tileDictionary = new StringTilebaseDictionary();
            
            TileBase[] oreTiles = Resources.LoadAll<TileBase>("Ores");

            foreach (var tile in oreTiles)
            {
                tileDictionary.Add(tile.name, tile);
            }
        }

        return tileDictionary[name];
    }
    
}