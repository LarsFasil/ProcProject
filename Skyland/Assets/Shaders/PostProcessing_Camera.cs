using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessing_Camera : MonoBehaviour
{
    [Range(1.0f, 800.0f)]
    public float Multiplication_Factor = 1;
    public Material material;
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        material.SetFloat("_MultFact", Multiplication_Factor);
        material.SetFloat("_Width", source.width);
        material.SetFloat("_Height", source.height);

        Graphics.Blit(source, destination, material);
    }
}
