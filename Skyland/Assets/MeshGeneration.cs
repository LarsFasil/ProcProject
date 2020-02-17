using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGeneration : MonoBehaviour
{
    Mesh mesh;
    Vector3[] v3A_vertices;
    int[] iA_triangles;

    MeshCollider meshcollider;

    [Range(0.0f, 10.0f)]
    public float perlinMultiplier, mountainMult;
    [Range(0.0f, 1.0f)]
    public float perlinreductor;
    [Range(0, 100)]
    public int mountainPercentage;

    public bool refresh = false;
    public bool gizmos = false;
    int i_sizeX, i_sizeY, i_chunkX, i_chunkY;

    public float yield;

    public void Init(int sx, int sy, int cx, int cy, float scale)
    {
        i_sizeX = sx;
        i_sizeY = sy;

        i_chunkX = cx;
        i_chunkY = cy;

        mountainMult = 0;

        transform.localScale = new Vector3(scale, scale, scale);

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        meshcollider = GetComponent<MeshCollider>();

        CreateMesh();
        UpdateMesh();
    }

    void CreateMesh()
    {
        v3A_vertices = new Vector3[(i_sizeX + 1) * (i_sizeY + 1)];
        float p;
        float emX = 0;
        float emY = 0;
        long i = 0;

        float percentageX = i_sizeX / 100 * mountainPercentage;
        float percentageY = i_sizeY / 100 * mountainPercentage;

        for (int y = 0 + (i_sizeY*i_chunkY); y <= i_sizeY + (i_sizeY * i_chunkY); y++)
        {
            for (int x = 0 + (i_sizeX * i_chunkX); x <= i_sizeX + +(i_sizeX * i_chunkX); x++)
            {
                //if (x <= percentageX || x >= (i_sizeX - percentageX) || y <= percentageY || y >= (i_sizeY - percentageY))
                //{
                //    em = mountainMult;
                //}

                if (x < percentageX)
                {
                    emX = ((x - percentageX) * -1) / percentageX * mountainMult;
                }
                if (x > (i_sizeX - percentageX))
                {
                    emX = (x - (i_sizeX - percentageX)) / percentageX * mountainMult;
                }


                if (y < percentageY)
                {
                    emY = ((y - percentageY) * -1) / percentageY * mountainMult;
                }
                if (y > (i_sizeY - percentageY))
                {
                    emY = (y - (i_sizeY - percentageY)) / percentageY * mountainMult;
                }

                if (emX <= emY)
                {
                    emX = emY;
                }

                if (emX < 1)
                {
                    emX = 1;
                }
                p = Mathf.PerlinNoise(x * perlinreductor, y * perlinreductor) * (emX * perlinMultiplier) * (perlinMultiplier); //* (Mathf.Sin(Time.time) + 1));
                v3A_vertices[i] = new Vector3(x, p, y);
                emX = 0;
                emY = 0;
                i++;

                //yield return new WaitForSeconds(yield);
            }
            //Debug.Log(i);
        }

        iA_triangles = new int[i_sizeX * i_sizeY * 6];

        int vert = 0;
        int tris = 0;

        for (int y = 0; y < i_sizeY; y++)
        {
            for (int x = 0; x < i_sizeX; x++)
            {
                iA_triangles[tris + 0] = vert + 0;
                iA_triangles[tris + 1] = vert + 1 + i_sizeX;
                iA_triangles[tris + 2] = vert + 1;

                iA_triangles[tris + 3] = vert + 1;
                iA_triangles[tris + 4] = vert + 1 + i_sizeX;
                iA_triangles[tris + 5] = vert + 2 + i_sizeX;

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

        meshcollider.sharedMesh = mesh;
    }

    private void OnDrawGizmos()
    {
        if (gizmos)
        {
            if (v3A_vertices == null)
            {
                return;
            }
            else
            {
                for (int i = 0; i < v3A_vertices.Length; i++)
                {
                    Gizmos.DrawSphere(v3A_vertices[i], .1f);
                }
            }
        }

    }
}
