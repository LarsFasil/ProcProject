using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    TerrainManager cs_terrainManager;
    public GameObject go_meshCombiner;
    MeshCombiner cs_meshCombiner;
    public int StoredChunks;
    public bool updateMesh = false;
    public bool CombineMesh = false;

    void Start()
    {
        cs_meshCombiner = go_meshCombiner.GetComponent<MeshCombiner>();
        cs_terrainManager = GetComponent<TerrainManager>();
    }

    void Update()
    {
        StoredChunks = cs_terrainManager.dict_VisitedChunks.Count;
        if (Input.GetKeyDown(KeyCode.D))
        {
            staticbatching();
        }
        if (updateMesh)
        {
            cs_terrainManager.ReloadChunks();
            updateMesh = false;
        }
        if (CombineMesh)
        {
            cs_meshCombiner.CombineMeshes();
            CombineMesh = false;
        }
        cs_terrainManager.TrackPlayer();
    }

    void staticbatching()
    {
        GameObject[] gos = new GameObject[go_meshCombiner.transform.childCount];
        for (int i = 0; i < go_meshCombiner.transform.childCount; i++)
        {
            gos[i] = go_meshCombiner.transform.GetChild(i).gameObject;
        }
        StaticBatchingUtility.Combine(gos,go_meshCombiner);
    }
}
