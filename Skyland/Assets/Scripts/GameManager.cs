using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    TerrainManager cs_terrainManager;
    public int StoredChunks;
    public bool Reload = false;

    void Start()
    {
        cs_terrainManager = GetComponent<TerrainManager>();
    }

    void Update()
    {
        StoredChunks = cs_terrainManager.dict_VisitedChunks.Count;
        if (Reload)
        {
            cs_terrainManager.ReloadChunks();
        }
        cs_terrainManager.TrackPlayer();
    }

}
