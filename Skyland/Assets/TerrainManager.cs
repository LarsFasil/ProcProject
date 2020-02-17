using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public GameObject go_dessert1;
    public Transform tf_player, tf_chunkParent;

    public int i_terrainSizeX;
    public int i_terrainSizeY;
    public float f_scale;

    public Vector2Int v2_playerChunk;
    GameObject[] goA_playerZone;

    Dictionary<Vector2Int, GameObject> dict_VisitedChunks = new Dictionary<Vector2Int, GameObject>();


    void Start()
    {
        v2_playerChunk = new Vector2Int(0, 0);
        goA_playerZone = new GameObject[9];

        MakeInitialZone();
    }

    void Update()
    {
        TrackPlayer();
    }

    void Move(short dir)
    {
        int[] North = { 0, 1, 2 };
        int[] East = { 0, 3, 6 };
        int[] South = { 6, 7, 8 };
        int[] West = { 2, 5, 8 };

        switch (dir)
        {
            case 1:
                UpdatePlayerZone(North, North.Length);
                break;
            case 2:          
                UpdatePlayerZone(East, 1);
                break;
            case 3:
                UpdatePlayerZone(South, -South.Length);
                break;
            case 4:
                UpdatePlayerZone(West, -1);
                break;
        }
        LoadDir(dir);
    }

    void UpdatePlayerZone(int[] code, int increment)
    {
        for (int i = 0; i < code.Length; i++)
        {
            goA_playerZone[code[i]].SetActive(false);
            goA_playerZone[code[i]] = goA_playerZone[code[i] + increment];
            goA_playerZone[code[i] + increment] = goA_playerZone[code[i] + (increment * 2)];
        }
    }

    void TrackPlayer()
    {
        float ppX = tf_player.position.x;
        float ppY = tf_player.position.z;

        if (ppY > (v2_playerChunk.y + 1) * i_terrainSizeY * f_scale)
        {
            // North
            v2_playerChunk.y++;
            Move(1);
        }
        if (ppX > (v2_playerChunk.x + 1) * i_terrainSizeX * f_scale)
        {
            // East
            v2_playerChunk.x++;
            Move(2);
        }
        if (ppY < v2_playerChunk.y * i_terrainSizeY * f_scale)
        {
            // South
            v2_playerChunk.y--;
            Move(3);
        }
        if (ppX < (v2_playerChunk.x) * i_terrainSizeX * f_scale)
        {
            // West
            v2_playerChunk.x--;
            Move(4);
        }
    }

    void MakeInitialZone()
    {
        //int initialChunkGridSize = 5;
        
        //// Initialize first x amount of chunks
        //for (int y = -initialChunkGridSize; y < initialChunkGridSize; y++)
        //{
        //    for (int x = -initialChunkGridSize; x < initialChunkGridSize; x++)
        //    {
        //        InstandGO(x, y).SetActive(false);
        //    }
        //}

        goA_playerZone[3] = InstandGO(-1, 0);
        goA_playerZone[4] = InstandGO(0, 0);
        goA_playerZone[5] = InstandGO(1, 0);

        LoadDir(1);
        LoadDir(3);
    }

    GameObject InstandGO(int x, int y)
    {
        Vector2Int chunk = new Vector2Int(x, y);
        if (!dict_VisitedChunks.ContainsKey(chunk))
        {
            GameObject mesh = Instantiate(go_dessert1, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            mesh.GetComponent<MeshGeneration>().Init(i_terrainSizeX, i_terrainSizeY, x, y, f_scale);

            // Clean up Hierarchy
            mesh.transform.parent = tf_chunkParent;

            // Update List
            dict_VisitedChunks.Add(chunk, mesh);

            return mesh;
        }

        if (!dict_VisitedChunks[chunk].activeSelf)
        {
            dict_VisitedChunks[chunk].SetActive(true);
        }
        return dict_VisitedChunks[chunk];
    }

    void LoadDir(int dir)
    {
        switch (dir)
        {
            case 1:
                goA_playerZone[6] = InstandGO(v2_playerChunk.x - 1, v2_playerChunk.y + 1);
                goA_playerZone[7] = InstandGO(v2_playerChunk.x, v2_playerChunk.y + 1);
                goA_playerZone[8] = InstandGO(v2_playerChunk.x + 1, v2_playerChunk.y + 1);
                break;

            case 2:
                goA_playerZone[2] = InstandGO(v2_playerChunk.x + 1, v2_playerChunk.y - 1);
                goA_playerZone[5] = InstandGO(v2_playerChunk.x + 1, v2_playerChunk.y);
                goA_playerZone[8] = InstandGO(v2_playerChunk.x + 1, v2_playerChunk.y + 1);
                break;

            case 3:
                goA_playerZone[0] = InstandGO(v2_playerChunk.x - 1, v2_playerChunk.y - 1);
                goA_playerZone[1] = InstandGO(v2_playerChunk.x, v2_playerChunk.y - 1);
                goA_playerZone[2] = InstandGO(v2_playerChunk.x + 1, v2_playerChunk.y - 1);
                break;

            case 4:
                goA_playerZone[0] = InstandGO(v2_playerChunk.x - 1, v2_playerChunk.y - 1);
                goA_playerZone[3] = InstandGO(v2_playerChunk.x - 1, v2_playerChunk.y);
                goA_playerZone[6] = InstandGO(v2_playerChunk.x - 1, v2_playerChunk.y + 1);
                break;

            default:
                break;
        }
    }
}
