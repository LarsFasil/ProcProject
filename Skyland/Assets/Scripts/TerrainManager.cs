using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject go_terrain;
    public GameObject go_tree;
    public GameObject go_playerPrefab;
    public Transform tf_chunkParent;

    //Terrain
    int i_chunkSize;
    float i_chunkRatio;
    int i_worldOffset;

    public float f_chunkScale;

    //Player
    public GameObject go_player;
    int i_playerzoneSize;
    float f_playerStartHeight;
    Vector2Int v2_playerStartingchunk;

    [Header("Other")]
    bool b_saveMeshes = true;
    public Vector2Int v2_playerChunk;
    MeshVars cs_meshVars;

    Transform tf_player;
    GameObject[] goA_playerZone;
    public Dictionary<Vector2Int, GameObject> dict_VisitedChunks = new Dictionary<Vector2Int, GameObject>();

    enum direction { North, East, South, West };

    void Start()
    {
        cs_meshVars = GetComponent<MeshVars>();
        init();
    }

    void init()
    {
        GetMeshVars();
        f_chunkScale = i_chunkRatio / i_chunkSize;
        if (i_playerzoneSize % 2 == 0)
        {
            i_playerzoneSize++;
        }
        InitializePlayer();
        MakeInitialZone();
    }

    // Tracks the player every frame to check if it left the playerchunk
    public void TrackPlayer()
    {
        float ppX = tf_player.position.x;
        float ppY = tf_player.position.z;

        if (ppY > ChunkToPos(v2_playerChunk.x, v2_playerChunk.y + 1, true).y)
        {
            // North
            UpdateZone(direction.North);
        }
        if (ppX > ChunkToPos(v2_playerChunk.x + 1, v2_playerChunk.y, true).x)
        {
            // East
            UpdateZone(direction.East);
        }
        if (ppY < ChunkToPos(v2_playerChunk.x, v2_playerChunk.y, true).y)
        {
            // South
            UpdateZone(direction.South);
        }
        if (ppX < ChunkToPos(v2_playerChunk.x, v2_playerChunk.y, true).x)
        {
            // West
            UpdateZone(direction.West);
        }
    }

    void UpdateZone(direction dir)
    {
        int zoneRatio = (int)(i_playerzoneSize * .5f - .5f);
        Vector2Int newChunk;
        switch (dir)
        {
            case direction.North:
                // Shift contents of all zones 1 zone down starting at the bottem left and skipping the upper row.
                for (int i = 0; i < i_playerzoneSize; i++)
                {
                    ProcessLeftChunks(goA_playerZone[i]);
                    for (int j = 0; j < i_playerzoneSize - 1; j++)
                    {
                        goA_playerZone[(j * i_playerzoneSize) + i] = goA_playerZone[(i_playerzoneSize * (j + 1)) + i];
                    }
                }
                // Add the new row of chunks to the skipped upper zones, creating a new land. 
                for (int i = 0; i < i_playerzoneSize; i++)
                {
                    newChunk = new Vector2Int(v2_playerChunk.x - zoneRatio + i, v2_playerChunk.y + zoneRatio + 1);
                    goA_playerZone[((i_playerzoneSize * i_playerzoneSize) - i_playerzoneSize) + i] = InstandGO(newChunk.x, newChunk.y);
                }
                v2_playerChunk.y++;
                break;

            case direction.East:
                for (int j = 0; j < i_playerzoneSize; j++)
                {
                    ProcessLeftChunks(goA_playerZone[j * i_playerzoneSize]);
                    for (int i = 0; i < i_playerzoneSize - 1; i++)
                    {
                        goA_playerZone[(j * i_playerzoneSize) + i] = goA_playerZone[(j * i_playerzoneSize) + i + 1];
                    }
                }
                // New row
                for (int i = 0; i < i_playerzoneSize; i++)
                {
                    newChunk = new Vector2Int(v2_playerChunk.x + zoneRatio + 1, v2_playerChunk.y - zoneRatio + i);
                    goA_playerZone[(i_playerzoneSize - 1) + (i * i_playerzoneSize)] = InstandGO(newChunk.x, newChunk.y);
                }
                v2_playerChunk.x++;
                break;

            case direction.South:
                for (int i = 0; i < i_playerzoneSize; i++)
                {
                    ProcessLeftChunks(goA_playerZone[((i_playerzoneSize * i_playerzoneSize) - i_playerzoneSize) + i]);
                    for (int j = i_playerzoneSize - 1; j > 0; j--)
                    {
                        goA_playerZone[(j * i_playerzoneSize) + i] = goA_playerZone[(i_playerzoneSize * (j - 1)) + i];
                    }
                }
                // New row
                for (int i = 0; i < i_playerzoneSize; i++)
                {
                    newChunk = new Vector2Int(v2_playerChunk.x - zoneRatio + i, v2_playerChunk.y - zoneRatio - 1);
                    goA_playerZone[i] = InstandGO(newChunk.x, newChunk.y);
                }
                v2_playerChunk.y--;
                break;

            case direction.West:
                for (int j = 0; j < i_playerzoneSize; j++)
                {
                    ProcessLeftChunks(goA_playerZone[(i_playerzoneSize - 1) + (j * i_playerzoneSize)]);
                    for (int i = i_playerzoneSize - 1; i > 0; i--)
                    {
                        goA_playerZone[(j * i_playerzoneSize) + i] = goA_playerZone[(j * i_playerzoneSize) + i - 1];
                    }
                }
                // New row
                for (int i = 0; i < i_playerzoneSize; i++)
                {
                    newChunk = new Vector2Int(v2_playerChunk.x - zoneRatio - 1, v2_playerChunk.y - zoneRatio + i);
                    goA_playerZone[i * i_playerzoneSize] = InstandGO(newChunk.x, newChunk.y);
                }
                v2_playerChunk.x--;
                break;
        }
    }

    void ProcessLeftChunks(GameObject chunk)
    {
        if (b_saveMeshes)
        {
            chunk.SetActive(false);
        }
        else
        {
            Destroy(chunk);
        }
    }

    void InitializePlayer()
    {
        // Instantiate the player on the startingchunk position.
        go_player = Instantiate(go_playerPrefab, ChunkToPos(v2_playerStartingchunk, true), Quaternion.identity) as GameObject;
        tf_player = go_player.transform;

        // Set the playerchunk to the players current chunk.
        v2_playerChunk = v2_playerStartingchunk;
    }

    void MakeInitialZone()
    {
        goA_playerZone = new GameObject[i_playerzoneSize * i_playerzoneSize];
        int zoneRatio = (int)((i_playerzoneSize * .5f) - .5f);

        // Fill up entire playerzone with the right chunks.
        for (int y = 0; y < i_playerzoneSize; y++)
        {
            for (int x = 0; x < i_playerzoneSize; x++)
            {
                goA_playerZone[x + (y * i_playerzoneSize)] = InstandGO(v2_playerStartingchunk.x + x - zoneRatio, v2_playerStartingchunk.y + y - zoneRatio);
            }
        }
    }

    GameObject InstandGO(int x, int y)
    {
        Vector2Int chunk = new Vector2Int(x, y);
        if ((b_saveMeshes && !dict_VisitedChunks.ContainsKey(chunk)) || !b_saveMeshes)
        {
            // Make chunk gameobject and use meshgeneration to create the mesh
            GameObject go_chunk = Instantiate(go_terrain, new Vector3((i_chunkSize * f_chunkScale * (x + i_worldOffset)), 0, (i_chunkSize * f_chunkScale * (y + i_worldOffset))), Quaternion.identity) as GameObject;
            MeshFilter mf_chunk = go_chunk.GetComponent<MeshFilter>();
            new MeshGeneration(x, y, cs_meshVars, mf_chunk, go_chunk.GetComponent<MeshCollider>());

            // Plant trees
            PlantTrees(mf_chunk);

            // Clean up Hierarchy
            go_chunk.transform.parent = tf_chunkParent;

            if (b_saveMeshes)
            {
                // Update dictionairy of visited chunks. 
                dict_VisitedChunks.Add(chunk, go_chunk);
            }

            return go_chunk;
        }

        if (!dict_VisitedChunks[chunk].activeSelf)
        {
            dict_VisitedChunks[chunk].SetActive(true);
        }
        return dict_VisitedChunks[chunk];
    }

    Vector3 ChunkToPos(Vector2Int chunk, bool WorldOffset)
    {
        Vector3 pos;
        float calcOffset = i_chunkSize * f_chunkScale * i_worldOffset;

        pos = new Vector3(chunk.x * i_chunkSize * f_chunkScale, f_playerStartHeight, chunk.y * i_chunkSize * f_chunkScale);

        if (WorldOffset)
        {
            pos.x += calcOffset;
            pos.z += calcOffset;
        }
        return pos;
    }
    Vector2 ChunkToPos(float chunkX, float chunkY, bool WorldOffset)
    {
        Vector2 pos;
        float calcOffset = i_chunkSize * f_chunkScale * i_worldOffset;

        pos = new Vector2(chunkX * i_chunkSize * f_chunkScale, chunkY * i_chunkSize * f_chunkScale);

        if (WorldOffset)
        {
            pos.x += calcOffset;
            pos.y += calcOffset;
        }
        return pos;
    }

    void PlantTrees(MeshFilter mf)
    {
        foreach (Vector3 i in mf.mesh.vertices)
        {
            if (i.y.ToString().EndsWith("109"))
            //if (Random.value > .995)
            {
                GameObject tree = Instantiate(go_tree, mf.transform.position + i, Quaternion.identity) as GameObject;
                tree.transform.parent = mf.transform;

                tree.transform.Rotate(0, Random.value * 100, 0, Space.Self);
                tree.transform.localScale *= Random.value + 1.5f;
                //Debug.Log(i.y.ToString());
                if (Random.value > .9999)
                {
                    tree.transform.localScale *= 5;
                }
                tree.transform.position += Vector3.up * -2;
            }

        }
    }

    public void ReloadChunks()
    {
        foreach (KeyValuePair<Vector2Int, GameObject> kvp in dict_VisitedChunks)
        {
            new MeshGeneration(kvp.Key.x, kvp.Key.y, cs_meshVars, kvp.Value.GetComponent<MeshFilter>(), kvp.Value.GetComponent<MeshCollider>());
        }
        GetMeshVars();
        f_chunkScale = i_chunkRatio / i_chunkSize;
    }
    //public void ReloadChunks()
    //{
    //    foreach (KeyValuePair<Vector2Int, GameObject> kvp in dict_VisitedChunks)
    //    {
    //        //kvp.Value.GetComponent<MeshGeneration>().Init(i_terrainSizeX, i_chunkSize, f_scaleMeter);
    //        Object.Destroy(kvp.Value);
    //    }
    //    dict_VisitedChunks.Clear();
    //    Destroy(tf_player.gameObject);
    //    init();
    //}
    void GetMeshVars()
    {
        i_chunkSize = cs_meshVars.i_chunkSize;
        i_chunkRatio = cs_meshVars.i_chunkRatio;
        i_worldOffset = cs_meshVars.i_worldOffset;

        i_playerzoneSize = cs_meshVars.i_playerzoneSize;
        f_playerStartHeight = cs_meshVars.f_playerStartHeight;
        v2_playerStartingchunk = cs_meshVars.v2_playerStartingchunk;

        b_saveMeshes = cs_meshVars.b_saveMeshes;
    }
}
