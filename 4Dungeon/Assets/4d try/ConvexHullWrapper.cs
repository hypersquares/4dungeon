using MIConvexHull;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ConvexHull wrapper class
public class ConvexHullWrapper : MonoBehaviour
{
    public static Mesh ConstructMesh(List<Vector4> vertices4d, Transform4D transform)
    {
        Vertex[] vertices = new Vertex[vertices4d.Count];
        for (int  i = 0; i < vertices.Length;  i++)
        {
            vertices[i] = transform.ProjectTo3D(vertices4d[i]);
        }

        if (vertices.Length < 4)
        {
            Debug.LogError("Not enough vertices to construct a convex hull.");
            return null;
        }

        var convexHullCreation = MIConvexHull.ConvexHull.Create(vertices);
        if (convexHullCreation.Outcome != MIConvexHull.ConvexHullCreationResultOutcome.Success)
        {
            Debug.LogError("Convex hull creation failed: " + convexHullCreation.ErrorMessage);
            return null;
        }

        var result = convexHullCreation.Result;

        List<Vector3> meshVertices = new List<Vector3>();
        List<int> meshTriangles = new List<int>();

        foreach (var face in result.Faces)
        {
            var indices = face.Vertices.Select(v => System.Array.IndexOf(vertices, v)).ToArray();
            if (indices.Length < 3) continue;
            // Triangulate the face (assuming it's a convex polygon)
            for (int i = 1; i < indices.Length - 1; i++)
            {
                meshTriangles.Add(meshVertices.Count + 0);
                meshTriangles.Add(meshVertices.Count + i);
                meshTriangles.Add(meshVertices.Count + i + 1);
            }
            foreach (var index in indices)
            {
                meshVertices.Add(new Vector3((float)vertices[index].Position[0], (float)vertices[index].Position[1], (float)vertices[index].Position[2]));
            }
        }

        Mesh mesh = new Mesh();
        mesh.SetVertices(meshVertices);
        mesh.SetTriangles(meshTriangles, 0);
        mesh.RecalculateNormals();
        return mesh;
    }
}
