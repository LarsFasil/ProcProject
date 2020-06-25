using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    TerrainManager cs_terrainManager;
    public int StoredChunks;
    public bool updateMesh = false;

    void Start()
    {
        cs_terrainManager = GetComponent<TerrainManager>();
    }

    void Update()
    {
        StoredChunks = cs_terrainManager.dict_VisitedChunks.Count;
        if (updateMesh)
        {
            cs_terrainManager.ReloadChunks();
            updateMesh = false;
        }
        cs_terrainManager.TrackPlayer();
    }

}
