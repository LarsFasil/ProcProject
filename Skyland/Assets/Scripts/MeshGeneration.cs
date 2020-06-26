using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGeneration : MonoBehaviour
{
    Mesh mesh;
    Vector3[] v3A_vertices;
    int[] iA_triangles;
    Vector2[] v2A_uv;

    MeshCollider meshcollider;
    MeshVars cs_meshVars;

    float perlinMultiplier;
    float perlinMultiplier2;
    float perlinreductor;
    int mountainPercentage, i_seed, i_octaves;

    int i_chunkSize, i_chunkX, i_chunkY;
    float f_scale;

    public MeshGeneration(int cx, int cy, MeshVars MV, MeshFilter MF, MeshCollider MC)
    {
        cs_meshVars = MV;
        i_chunkX = cx + cs_meshVars.i_worldOffset;
        i_chunkY = cy + cs_meshVars.i_worldOffset;

        mesh = new Mesh();

        MF.mesh = mesh;
        meshcollider = MC;
        //GetComponent<MeshFilter>().mesh = mesh;
        //meshcollider = GetComponent<MeshCollider>();

        GetMeshVars();

        CreateMesh();
        UpdateMesh();
    }

    void CreateMesh()
    {
        v3A_vertices = new Vector3[(i_chunkSize + 1) * (i_chunkSize + 1)];
        v2A_uv = new Vector2[(i_chunkSize + 1) * (i_chunkSize + 1)];
        long i = 0;

        for (int y = 0; y <= i_chunkSize; y++)
        {
            for (int x = 0; x <= i_chunkSize; x++)
            {
                float px = (i_chunkSize * i_chunkX + x) * perlinreductor;
                float py = (i_chunkSize * i_chunkY + y) * perlinreductor;

                float p = Mathf.PerlinNoise(px, py) * (perlinMultiplier * perlinMultiplier2);    

                v3A_vertices[i] = new Vector3(x * f_scale, p, y * f_scale);
                v2A_uv[i] = new Vector2(x / (float)i_chunkSize, y / (float)i_chunkSize);
                i++;
            }
        }

        iA_triangles = new int[i_chunkSize * i_chunkSize * 6];

        int vert = 0;
        int tris = 0;

        for (int y = 0; y < i_chunkSize; y++)
        {
            for (int x = 0; x < i_chunkSize; x++)
            {
                iA_triangles[tris + 0] = vert + 0;
                iA_triangles[tris + 1] = vert + 1 + i_chunkSize;
                iA_triangles[tris + 2] = vert + 1;

                iA_triangles[tris + 3] = vert + 1;
                iA_triangles[tris + 4] = vert + 1 + i_chunkSize;
                iA_triangles[tris + 5] = vert + 2 + i_chunkSize;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = v3A_vertices;
        mesh.triangles = iA_triangles;

        mesh.RecalculateNormals();
        mesh.uv = v2A_uv;

        meshcollider.sharedMesh = mesh;
    }

    void GetMeshVars()
    {
        perlinMultiplier = cs_meshVars.perlinMultiplier;
        perlinMultiplier2 = cs_meshVars.perlinMultiplier2;
        perlinreductor = cs_meshVars.perlinreductor;
        i_seed = cs_meshVars.i_seed;

        i_chunkSize = cs_meshVars.i_chunkSize;
        f_scale = cs_meshVars.i_chunkRatio / cs_meshVars.i_chunkSize;
    }
}
