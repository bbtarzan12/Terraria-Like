using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace XNode.Noise.PostProcessing
{
 
    [Serializable]
    public class DenoiseNode : NoiseNode
    {
        [Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public NoiseNode input;
        [Output] public NoiseNode output;
        public int minRoomSize = 5;

        JobHandle jobHandle;
        
        public override Texture2D GetTexture()
        {
            Texture2D texture = ((NoiseNode) GetPort(nameof(input)).Connection.node).GetTexture();
            Vector2Int size = new Vector2Int(texture.width, texture.height);
            Texture = TextureMaker.Generate(texture);
            Color[] colors = Texture.GetPixels();
            List<List<Vector2Int>> regions = new List<List<Vector2Int>>();
            HashSet<int> visited = new HashSet<int>();
            for (int i = 0; i < colors.Length; i++)
            {
                if (colors[i].maxColorComponent <= 0)
                    continue;
                
                Vector2Int coord = new Vector2Int(i % size.x, i / size.x );
                
                if(visited.Contains(i))
                    continue;

                List<Vector2Int> region = CheckRegion(colors, coord, size, ref visited);
                if(region.Count > 0)
                    regions.Add(region);
            }

            foreach (List<Vector2Int> region in regions)
            {
                if(region.Count > minRoomSize)
                    continue;

                foreach (Vector2Int coord in region)
                {
                    colors[coord.x + coord.y * size.x] = Color.black;
                }
            }
            
            Texture.SetPixels(colors);
            Texture.Apply();
            return Texture;
        }

        List<Vector2Int> CheckRegion(Color[] map, Vector2Int coord, Vector2Int size, ref HashSet<int> visited)
        {
            List<Vector2Int> tiles = new List<Vector2Int>();
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            queue.Enqueue(coord);
            while (queue.Count > 0)
            {
                Vector2Int tile = queue.Dequeue();
                tiles.Add(tile);
                
                for (int x = tile.x - 1; x <= tile.x + 1; x++)
                {
                    for (int y = tile.y - 1; y <= tile.y + 1; y++)
                    {
                        if ((x < 0 || x >= size.x || y < 0 || y >= size.y) || (x != tile.x && y != tile.y)) continue;

                        int index = x + y * size.x;
                        Vector2Int neighborTile = new Vector2Int(x, y);
                        
                        if (visited.Contains(index)) continue;
                        if(map[index].maxColorComponent <= 0) continue;
                        
                        visited.Add(index);
                        queue.Enqueue(neighborTile);
                    }
                }
            }

            return tiles;
        }
        
        public override Texture2D GenerateTexture()
        {
            if (!Dirty)
                return HasTexture ? Texture : GenerateTexture();

            NoiseNode inputNoise = GetInputValue<NoiseNode>("input");
            
            if (inputNoise == null)
                return Texture2D.whiteTexture;

            Texture2D inputTexture = !inputNoise.HasTexture ? inputNoise.GenerateTexture() : inputNoise.Texture;
            
            Vector2Int size = new Vector2Int(inputTexture.width, inputTexture.height);
            Texture = TextureMaker.Generate(inputTexture);
            Color[] colors = Texture.GetPixels();
            List<List<Vector2Int>> regions = new List<List<Vector2Int>>();
            HashSet<int> visited = new HashSet<int>();
            for (int i = 0; i < colors.Length; i++)
            {
                if (colors[i].maxColorComponent <= 0)
                    continue;
                
                Vector2Int coord = new Vector2Int(i % size.x, i / size.x );
                
                if(visited.Contains(i))
                    continue;

                List<Vector2Int> region = CheckRegion(colors, coord, size, ref visited);
                if(region.Count > 0)
                    regions.Add(region);
            }

            foreach (List<Vector2Int> region in regions)
            {
                if(region.Count > minRoomSize)
                    continue;

                foreach (Vector2Int coord in region)
                {
                    colors[coord.x + coord.y * size.x] = Color.black;
                }
            }
            
            Texture.SetPixels(colors);
            Texture.Apply();
            
            
            GetPort(nameof(output))?.GetConnections()?.ForEach(f => ((NoiseNode)f.node)?.SetTextureDirty());
            Dirty = false;

            return Texture;
        }
    }

}