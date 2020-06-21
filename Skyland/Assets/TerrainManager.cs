using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject go_dessert1;
    public GameObject go_playerPrefab;
    public Transform tf_chunkParent;

    [Header("Terrain")]
    public int i_terrainSizeX;
    public int i_terrainSizeY;
    public float f_scaleMeter;
    public int i_worldOffset;

    [Header("Player")]
    public int i_playerzoneSize;
    public float f_playerStartHeight;
    public Vector2Int v2_playerStartingchunk;

    [Header("Other")]
    public bool b_saveMeshes = true;
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
        if (i_playerzoneSize % 2 == 0)
        {
            i_playerzoneSize++;
        }
        initializePlayer();
        MakeInitialZone();
    }

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

    void initializePlayer()
    {
        // Instantiate the player on the startingchunk position.
        GameObject player = Instantiate(go_playerPrefab, ChunkToPos(v2_playerStartingchunk, true), Quaternion.identity) as GameObject;
        tf_player = player.transform;

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
            GameObject mesh = Instantiate(go_dessert1, new Vector3(0 + (i_terrainSizeX * f_scaleMeter * (x + i_worldOffset)), 0, 0 + (i_terrainSizeY * f_scaleMeter * (y + i_worldOffset))), Quaternion.identity) as GameObject;
            mesh.GetComponent<MeshGeneration>().Init(i_terrainSizeX, i_terrainSizeY, x + i_worldOffset, y + i_worldOffset, f_scaleMeter,cs_meshVars);

            // Clean up Hierarchy
            mesh.transform.parent = tf_chunkParent;

            if (b_saveMeshes)
            {
                // Update dictionairy of visited chunks. 
                dict_VisitedChunks.Add(chunk, mesh);
            }

            return mesh;
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
        float calcOffset = i_terrainSizeY * f_scaleMeter * i_worldOffset;

        pos = new Vector3(chunk.x * i_terrainSizeX * f_scaleMeter, f_playerStartHeight, chunk.y * i_terrainSizeY * f_scaleMeter);

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
        float calcOffset = i_terrainSizeY * f_scaleMeter * i_worldOffset;

        pos = new Vector2(chunkX * i_terrainSizeX * f_scaleMeter, chunkY * i_terrainSizeY * f_scaleMeter);

        if (WorldOffset)
        {
            pos.x += calcOffset;
            pos.y += calcOffset;
        }
        return pos;
    }

    public void ReloadChunks()
    {
        foreach (KeyValuePair<Vector2Int, GameObject> kvp in dict_VisitedChunks)
        {
            //kvp.Value.GetComponent<MeshGeneration>().Init(i_terrainSizeX, i_terrainSizeY, f_scaleMeter);
            Object.Destroy(kvp.Value);
        }
        dict_VisitedChunks.Clear();
        Destroy(tf_player.gameObject);
        init();
    }
}
