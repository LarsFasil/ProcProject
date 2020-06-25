using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshVars : MonoBehaviour
{
    [Header("Perlin noise")]
    [Range(0.0f, 200.0f)]
    public float perlinMultiplier;
    [Range(0.0f, 10f)]
    public float perlinMultiplier2;
    [Range(0.0f, 1.0f)]
    public float perlinreductor;
    public int i_seed;

    [Header("Terrain")]
    [Range(1,70)]
    public int i_chunkSize;
    public float i_chunkRatio;
    public int i_worldOffset;

    [Header("Player")]
    public int i_playerzoneSize;
    public float f_playerStartHeight;
    public Vector2Int v2_playerStartingchunk;

    [Header("Other")]
    public bool b_saveMeshes = true;
}
