using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
public class MeshGeneratorScript : MonoBehaviour
{
    // Reference a Mesh
    Mesh mesh;

    // Arrays for the vertices and triangles
    Vector3[] vertices;
    int[] triangles;
    Color[] colors;

    // Control the size of the generated mesh
    public int xSize = 1000;
    public int zSize = 1000;

    // Gradient for colouring the mesh
    public Gradient gradient;
    
    float minTerrainHeight;
    float maxTerrainHeight;

    // Generate the mesh
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateShape();
        UpdateMesh();
    }

    // Generate mesh until max xSize and zSize are reached
    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            // Use a Perlin Noise to generate random frequency and amplitude
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise(x * 0.2f, z * 0.2f) * 9f;
                vertices[i] = new Vector3(x, y, z);

                // clamps value
                if(y > maxTerrainHeight)
                {
                    maxTerrainHeight = y;
                }

                if (y < minTerrainHeight)
                {
                    minTerrainHeight = y;
                }

                i++;
            }
        }

        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;

        // Generates two triangles from each vertice making a quad
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;

            }
            //stops an extra triangle being produced when it changes rows
            vert++;
        }

        //Creates a colour based on the vertices height
        colors = new Color[vertices.Length];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            
            for (int x = 0; x <= xSize; x++)
            {
                float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);
                colors[i] = gradient.Evaluate(height);
                i++;
            }
        }

    }

    // Render the mesh and recalculate the normals 
    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateNormals();
    }

    // Use this for debugging vertice positions

    /* private void OnDrawGizmos()
    {
        if (vertices == null)
            return;

        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], .1f);

        }
    }
    */

}
