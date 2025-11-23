using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Cube4DInitializer : MonoBehaviour
{
	public Mesh4D mesh;

	[ContextMenu("Create Cube4D")]
	private void CreateCube4()
	{
		CreateCube4(mesh, -1, 1, -1, 1, -1, 1, -1, 1);
		Debug.Log("Initialized Cube4D with tesseract geometry!");
	}

	[ContextMenu("Create Tesseract")]
	private void CreateTesseract()
	{
		CreateTesseract(mesh, -1, 1, -1, 1, -1, 1, -1, 1);
		Debug.Log("Created Tesseract in 4D!");
	}

	[ContextMenu("Create Sphere4D")]
	private void CreateSphere4D()
	{
		CreateSphere4(mesh, radius: 1f, resolution: 6);
		Debug.Log("Created Sphere4D in 4D!");
	}

    public static void CreateCube4(
        Mesh4D mesh,
        float x0, float x1,
        float y0, float y1,
        float z0, float z1,
        float w0, float w1)
    {
        Vector4[] vertices = new Vector4[8 * 2];
        Edge[] edges = new Edge[12 * 2 + 8];

        // w = 0
        // Face down 
        vertices[0] = new Vector4(x0, y0, z0, w0);
        vertices[1] = new Vector4(x1, y0, z0, w0);
        edges[0] = new Edge(0, 1);

        vertices[2] = new Vector4(x0, y0, z1, w0);
        vertices[3] = new Vector4(x1, y0, z1, w0);
        edges[1] = new Edge(2, 3);

        edges[2] = new Edge(0, 2);
        edges[3] = new Edge(1, 3);

        // Face up
        vertices[4] = new Vector4(x0, y1, z0, w0);
        vertices[5] = new Vector4(x1, y1, z0, w0);
        edges[4] = new Edge(4, 5);

        vertices[6] = new Vector4(x0, y1, z1, w0);
        vertices[7] = new Vector4(x1, y1, z1, w0);
        edges[5] = new Edge(6, 7);

        edges[6] = new Edge(4, 6);
        edges[7] = new Edge(5, 7);

        // Connects the two faces
        edges[8] = new Edge(0, 4);
        edges[9] = new Edge(1, 5);
        edges[10] = new Edge(2, 6);
        edges[11] = new Edge(3, 7);

        // w = 0
        // Face down
        vertices[0 + 8] = new Vector4(x0, y0, z0, w1);
        vertices[1 + 8] = new Vector4(x1, y0, z0, w1);
        edges[0 + 12] = new Edge(0 + 8, 1 + 8);

        vertices[2 + 8] = new Vector4(x0, y0, z1, w1);
        vertices[3 + 8] = new Vector4(x1, y0, z1, w1);
        edges[1 + 12] = new Edge(2 + 8, 3 + 8);

        edges[2 + 12] = new Edge(0 + 8, 2 + 8);
        edges[3 + 12] = new Edge(1 + 8, 3 + 8);

        // Face up
        vertices[4 + 8] = new Vector4(x0, y1, z0, w1);
        vertices[5 + 8] = new Vector4(x1, y1, z0, w1);
        edges[4 + 12] = new Edge(4 + 8, 5 + 8);

        vertices[6 + 8] = new Vector4(x0, y1, z1, w1);
        vertices[7 + 8] = new Vector4(x1, y1, z1, w1);
        edges[5 + 12] = new Edge(6 + 8, 7 + 8);

        edges[6 + 12] = new Edge(4 + 8, 6 + 8);
        edges[7 + 12] = new Edge(5 + 8, 7 + 8);

        // Connects the two faces
        edges[8 + 12] = new Edge(0 + 8, 4 + 8);
        edges[9 + 12] = new Edge(1 + 8, 5 + 8);
        edges[10 + 12] = new Edge(2 + 8, 6 + 8);
        edges[11 + 12] = new Edge(3 + 8, 7 + 8);

        // Connects the two cubes
        edges[24] = new Edge(0, 0 + 8);
        edges[25] = new Edge(1, 1 + 8);
        edges[26] = new Edge(2, 2 + 8);
        edges[27] = new Edge(3, 3 + 8);
        edges[28] = new Edge(4, 4 + 8);
        edges[29] = new Edge(5, 5 + 8);
        edges[30] = new Edge(6, 6 + 8);
        edges[31] = new Edge(7, 7 + 8);

        // Copies the new geometry
        mesh.Vertices = vertices;
        mesh.Edges = edges;
    }

    public static void CreateTesseract(Mesh4D mesh,
        float x0, float x1,
        float y0, float y1,
        float z0, float z1,
        float w0, float w1)
    {
        // Tesseract: 16 vertices, 32 edges
        Vector4[] vertices = new Vector4[16];
        Edge[] edges = new Edge[32];

        // Generate all combinations of (x, y, z, w)
        int idx = 0;
        float[] xs = { x0, x1 };
        float[] ys = { y0, y1 };
        float[] zs = { z0, z1 };
        float[] ws = { w0, w1 };
        for (int i = 0; i < 2; i++)
            for (int j = 0; j < 2; j++)
                for (int k = 0; k < 2; k++)
                    for (int l = 0; l < 2; l++)
                        vertices[idx++] = new Vector4(xs[i], ys[j], zs[k], ws[l]);

        // Add edges between vertices that differ by one coordinate
        int edgeIdx = 0;
        for (int a = 0; a < 16; a++)
        {
            for (int b = a + 1; b < 16; b++)
            {
                int diff = 0;
                if (vertices[a].x != vertices[b].x) diff++;
                if (vertices[a].y != vertices[b].y) diff++;
                if (vertices[a].z != vertices[b].z) diff++;
                if (vertices[a].w != vertices[b].w) diff++;
                if (diff == 1)
                {
                    edges[edgeIdx++] = new Edge(a, b);
                    if (edgeIdx == 32) break;
                }
            }
            if (edgeIdx == 32) break;
        }

        mesh.Vertices = vertices;
        mesh.Edges = edges;
    }



    public static void CreateSphere4(Mesh4D mesh, float radius, int resolution)
    {
        List<Vector4> verts = new List<Vector4>();
        List<Edge> edges = new List<Edge>();

        // Generate vertices on 4D sphere
        for (int i = 0; i < resolution; i++)
        {
            float theta1 = Mathf.PI * i / (resolution - 1); // [0, π]
            for (int j = 0; j < resolution; j++)
            {
                float theta2 = Mathf.PI * j / (resolution - 1); // [0, π]
                for (int k = 0; k < resolution; k++)
                {
                    float theta3 = 2 * Mathf.PI * k / (resolution - 1); // [0, 2π]

                    float x = radius * Mathf.Cos(theta1);
                    float y = radius * Mathf.Sin(theta1) * Mathf.Cos(theta2);
                    float z = radius * Mathf.Sin(theta1) * Mathf.Sin(theta2) * Mathf.Cos(theta3);
                    float w = radius * Mathf.Sin(theta1) * Mathf.Sin(theta2) * Mathf.Sin(theta3);

                    verts.Add(new Vector4(x, y, z, w));
                }
            }
        }

        // Connect neighbors (simple wireframe grid)
        int stride2 = resolution;
        int stride3 = resolution * resolution;
        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                for (int k = 0; k < resolution; k++)
                {
                    int index = i * stride3 + j * stride2 + k;

                    if (i + 1 < resolution) edges.Add(new Edge(index, index + stride3));
                    if (j + 1 < resolution) edges.Add(new Edge(index, index + stride2));
                    if (k + 1 < resolution) edges.Add(new Edge(index, index + 1));
                }
            }
        }

        mesh.Vertices = verts.ToArray();
        mesh.Edges = edges.ToArray();
    }


    public static void CreateCubeSphereMorph(Mesh4D mesh, float radius, int resolution, float t)
    {
        List<Vector4> verts = new List<Vector4>();
        List<Edge> edges = new List<Edge>();

        // Build subdivided cube (resolution^3 grid of points on the cube surface)
        for (int i = 0; i < resolution; i++)
        {
            float x = Mathf.Lerp(-1, 1, (float)i / (resolution - 1));
            for (int j = 0; j < resolution; j++)
            {
                float y = Mathf.Lerp(-1, 1, (float)j / (resolution - 1));
                for (int k = 0; k < resolution; k++)
                {
                    float z = Mathf.Lerp(-1, 1, (float)k / (resolution - 1));
                    for (int l = 0; l < resolution; l++)
                    {
                        float w = Mathf.Lerp(-1, 1, (float)l / (resolution - 1));

                        // Base cube point
                        Vector4 cubePoint = new Vector4(x, y, z, w);

                        // Sphere point (normalize then scale)
                        Vector4 spherePoint = cubePoint.normalized * radius;

                        // Interpolated point
                        Vector4 v = Vector4.Lerp(cubePoint, spherePoint, t);

                        verts.Add(v);
                    }
                }
            }
        }

        // Edges: connect neighbors in the 4D grid (wireframe style)
        int stride1 = resolution;
        int stride2 = resolution * resolution;
        int stride3 = resolution * resolution * resolution;

        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                for (int k = 0; k < resolution; k++)
                {
                    for (int l = 0; l < resolution; l++)
                    {
                        int index = i * stride3 + j * stride2 + k * stride1 + l;

                        if (i + 1 < resolution) edges.Add(new Edge(index, index + stride3));
                        if (j + 1 < resolution) edges.Add(new Edge(index, index + stride2));
                        if (k + 1 < resolution) edges.Add(new Edge(index, index + stride1));
                        if (l + 1 < resolution) edges.Add(new Edge(index, index + 1));
                    }
                }
            }
        }

        mesh.Vertices = verts.ToArray();
        mesh.Edges = edges.ToArray();
    }
}
