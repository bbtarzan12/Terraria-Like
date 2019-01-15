using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NoiseGenerator : MonoBehaviour
{
    [SerializeField] Vector2Int mapsize;
    [SerializeField] float threshold;
    [SerializeField] Tile tile;

    [SerializeField] List<Noise> noises;

    public List<Noise> Noises => noises;
    public Vector2Int MapSize => mapsize;
    public Texture2D MapTexture => noises[noises.Count - 1].Texture;

    public void InitAllNoises()
    {
        if(noises == null)
            noises = new List<Noise>();

        for (int i = 0; i < noises.Count; i++)
        {
            var noise = noises[i];
            noise.Update(mapsize, i != 0 ? noises[i - 1].Texture : Texture2D.whiteTexture);
            noise.DrawTexture();
        }
    }

    public void RefreshAllTextures()
    {
        foreach (var noise in noises)
        {
            noise.SetDirty();
        }
    }

    public void RefreshAfter(int index)
    {
        for (int i = index; i < noises.Count; i++)
        {
            noises[i].SetDirty();
        }
    }
}