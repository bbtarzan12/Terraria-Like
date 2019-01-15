using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NoiseGenerator : MonoBehaviour
{
    [SerializeField] Vector2Int mapsize;
    [SerializeField] float threshold;
    [SerializeField] Tile tile;

    [SerializeField] List<INoise> noises;

    public List<INoise> Noises
    {
        get { return noises; }
        set { noises = value; }
    }
}