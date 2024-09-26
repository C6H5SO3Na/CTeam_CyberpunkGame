using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxVisual : MonoBehaviour
{
    // Start is called before the first frame update
    private MeshRenderer meshRenderer;
    private Material material;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.material;
    }

    void Update()
    {
        Color color = material.color;
        color.a = 0; // Set the alpha value
        material.color = color;
    }
}
