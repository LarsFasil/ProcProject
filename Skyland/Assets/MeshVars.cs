using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshVars : MonoBehaviour
{
    [Range(0.0f, 10.0f)]
    public float perlinMultiplier, mountainMult;
    [Range(0.0f, 1.0f)]
    public float perlinreductor;
    [Range(0, 100)]
    public int mountainPercentage;

    public bool gizmos = false;
}
