using System.Collections;
using System.Collections.Generic;
using static System.Runtime.InteropServices.Marshal;
using UnityEngine;
using Unity.VisualScripting;

public class GenerateGrass : MonoBehaviour
{
    public int mapSize;
    private int grassAmount = 10000;
    public float density = 1;

    public ComputeShader grassShader;
    private ComputeBuffer grassDataBuffer;

    public Mesh grassMesh;
    public Material grassMaterial;

    Bounds mapBounds;

    private ComputeBuffer argsBuffer;

    public Texture2D heightMap;
    public float displacementStrength = 1.0f;

    void Start()
    {
        grassAmount = (int)(mapSize * mapSize);

        mapBounds = new Bounds(Vector3.zero, new Vector3(-mapSize, mapSize * 2.0f, mapSize));

        // args
        uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
        args[0] = (uint)grassMesh.GetIndexCount(0);
        args[1] = (uint)(grassAmount);  // number of meshes
        args[2] = (uint)grassMesh.GetIndexStart(0);
        args[3] = (uint)grassMesh.GetBaseVertex(0);

        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        argsBuffer.SetData(args);

        // grass positions
        grassDataBuffer = new ComputeBuffer(mapSize * mapSize, 4*4); // 4 numbers in grass data struct
        int kernel = grassShader.FindKernel("InitialiseGrassPositions");

        grassShader.SetInt("_MapSize", mapSize);
        grassShader.SetFloat("_Density", density);
        grassShader.SetFloat("_DisplacementStrength", displacementStrength);
        grassShader.SetTexture(kernel, "_HeightMap", heightMap);
        grassShader.SetBuffer(kernel, "_GrassDataBuffer", grassDataBuffer);
        grassShader.Dispatch(kernel, Mathf.CeilToInt(mapSize / 8.0f), Mathf.CeilToInt(mapSize / 8.0f), 1);

        // shader
        grassMaterial.SetBuffer("GrassDataBuffer", grassDataBuffer);
    }

    void Update()
    {
        Graphics.DrawMeshInstancedIndirect(grassMesh, 0, grassMaterial, mapBounds, argsBuffer);
    }
}
