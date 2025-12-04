using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class TetrahedronReorderVertsTests
{
    private const float k_Epsilon = 0.0001f;

    /// <summary>
    /// Helper class to access the private ReorderVerts method
    /// </summary>
    private class TetrahedronTestHelper : Tetrahedron
    {
        public TetrahedronTestHelper() : base(new Vector4[] { Vector4.zero, Vector4.zero, Vector4.zero, Vector4.zero })
        {
        }

        public void TestReorderVerts(List<Vector3> verts)
        {
            // Use reflection to call the private method
            var method = typeof(Tetrahedron).GetMethod("ReorderVerts",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.Invoke(this, new object[] { verts });
        }
    }

    /// <summary>
    /// Checks if two 2D triangles (ignoring Z) intersect beyond their shared edge
    /// </summary>
    private bool DoTrianglesIntersect(Vector3 t1v0, Vector3 t1v1, Vector3 t1v2,
                                       Vector3 t2v0, Vector3 t2v1, Vector3 t2v2)
    {
        // For triangles (0,1,2) and (1,2,3), they share edge (1,2)
        // We want to check if vertex 0 and vertex 3 are on opposite sides of edge (1,2)
        // If they're on opposite sides, the triangles don't intersect (beyond their shared edge)

        // In this test context:
        // t1 = (0,1,2) and t2 = (1,2,3)
        // So t1v0=v0, t1v1=v1, t1v2=v2, t2v0=v1, t2v1=v2, t2v2=v3

        // The shared edge is (t1v1, t1v2) = (v1, v2) = (t2v0, t2v1)
        Vector3 sharedV1 = t1v1;
        Vector3 sharedV2 = t1v2;
        Vector3 vertex0 = t1v0;
        Vector3 vertex3 = t2v2;

        // Check if vertex0 and vertex3 are on opposite sides of the line through sharedV1-sharedV2
        Vector3 edgeDir = sharedV2 - sharedV1;
        Vector3 perpendicular = new Vector3(-edgeDir.y, edgeDir.x, 0); // 2D perpendicular

        float dot0 = Vector3.Dot(vertex0 - sharedV1, perpendicular);
        float dot3 = Vector3.Dot(vertex3 - sharedV1, perpendicular);

        // If signs are different, they're on opposite sides (good - no intersection)
        // If signs are the same, they're on the same side (bad - triangles overlap/intersect)
        return Mathf.Sign(dot0) == Mathf.Sign(dot3) && Mathf.Abs(dot0) > k_Epsilon && Mathf.Abs(dot3) > k_Epsilon;
    }

    /// <summary>
    /// Verifies that triangles (0,1,2) and (1,2,3) don't intersect
    /// </summary>
    private bool AreTrianglesNonIntersecting(List<Vector3> verts)
    {
        Assert.AreEqual(4, verts.Count, "Expected exactly 4 vertices");

        // Triangle 1: verts[0], verts[1], verts[2]
        // Triangle 2: verts[1], verts[2], verts[3]
        bool intersects = DoTrianglesIntersect(
            verts[0], verts[1], verts[2],
            verts[1], verts[2], verts[3]
        );

        return !intersects;
    }

    [Test]
    public void ReorderVerts_ConvexQuadInOrder_ProducesNonIntersectingTriangles()
    {
        // Convex quad: square in XY plane
        // 3---2
        // |   |
        // 0---1
        var verts = new List<Vector3>
        {
            new Vector3(0, 0, 0),  // bottom-left
            new Vector3(1, 0, 0),  // bottom-right
            new Vector3(1, 1, 0),  // top-right
            new Vector3(0, 1, 0)   // top-left
        };

        var helper = new TetrahedronTestHelper();
        helper.TestReorderVerts(verts);

        bool nonIntersecting = AreTrianglesNonIntersecting(verts);
        Assert.IsTrue(nonIntersecting,
            $"Triangles should be non-intersecting. Vertices after reorder: [{verts[0]}, {verts[1]}, {verts[2]}, {verts[3]}]");
    }

    [Test]
    public void ReorderVerts_ConvexQuadScrambledOrder1_ProducesNonIntersectingTriangles()
    {
        // Convex quad with vertices in scrambled order
        // Should form: 0-2 diagonal split
        var verts = new List<Vector3>
        {
            new Vector3(0, 0, 0),  // bottom-left
            new Vector3(1, 1, 0),  // top-right
            new Vector3(1, 0, 0),  // bottom-right
            new Vector3(0, 1, 0)   // top-left
        };

        var helper = new TetrahedronTestHelper();
        helper.TestReorderVerts(verts);

        bool nonIntersecting = AreTrianglesNonIntersecting(verts);
        Assert.IsTrue(nonIntersecting,
            $"Triangles should be non-intersecting. Vertices after reorder: [{verts[0]}, {verts[1]}, {verts[2]}, {verts[3]}]");
    }

    [Test]
    public void ReorderVerts_ConvexQuadScrambledOrder2_ProducesNonIntersectingTriangles()
    {
        // Another scrambled ordering
        var verts = new List<Vector3>
        {
            new Vector3(0, 0, 0),  // bottom-left
            new Vector3(0, 1, 0),  // top-left
            new Vector3(1, 1, 0),  // top-right
            new Vector3(1, 0, 0)   // bottom-right
        };

        var helper = new TetrahedronTestHelper();
        helper.TestReorderVerts(verts);

        bool nonIntersecting = AreTrianglesNonIntersecting(verts);
        Assert.IsTrue(nonIntersecting,
            $"Triangles should be non-intersecting. Vertices after reorder: [{verts[0]}, {verts[1]}, {verts[2]}, {verts[3]}]");
    }

    [Test]
    public void ReorderVerts_RectangularQuad_ProducesNonIntersectingTriangles()
    {
        // Rectangular quad (2:1 aspect ratio)
        var verts = new List<Vector3>
        {
            new Vector3(0, 0, 0),
            new Vector3(2, 0, 0),
            new Vector3(2, 1, 0),
            new Vector3(0, 1, 0)
        };

        var helper = new TetrahedronTestHelper();
        helper.TestReorderVerts(verts);

        bool nonIntersecting = AreTrianglesNonIntersecting(verts);
        Assert.IsTrue(nonIntersecting,
            $"Triangles should be non-intersecting. Vertices after reorder: [{verts[0]}, {verts[1]}, {verts[2]}, {verts[3]}]");
    }

    [Test]
    public void ReorderVerts_ConcaveQuad_ProducesNonIntersectingTriangles()
    {
        // Concave quad (arrow shape pointing right)
        //    2
        //   /|
        //  0-1
        //   \|
        //    3
        var verts = new List<Vector3>
        {
            new Vector3(0, 0.5f, 0),  // left middle
            new Vector3(1, 0.5f, 0),  // right middle
            new Vector3(1.5f, 1, 0),  // top right
            new Vector3(1.5f, 0, 0)   // bottom right
        };

        var helper = new TetrahedronTestHelper();
        helper.TestReorderVerts(verts);

        bool nonIntersecting = AreTrianglesNonIntersecting(verts);
        Assert.IsTrue(nonIntersecting,
            $"Triangles should be non-intersecting. Vertices after reorder: [{verts[0]}, {verts[1]}, {verts[2]}, {verts[3]}]");
    }

    [Test]
    public void ReorderVerts_DiamondShape_ProducesNonIntersectingTriangles()
    {
        // Diamond shape (rotated square)
        //     2
        //    / \
        //   0   1
        //    \ /
        //     3
        var verts = new List<Vector3>
        {
            new Vector3(0, 0.5f, 0),    // left
            new Vector3(1, 0.5f, 0),    // right
            new Vector3(0.5f, 1, 0),    // top
            new Vector3(0.5f, 0, 0)     // bottom
        };

        var helper = new TetrahedronTestHelper();
        helper.TestReorderVerts(verts);

        bool nonIntersecting = AreTrianglesNonIntersecting(verts);
        Assert.IsTrue(nonIntersecting,
            $"Triangles should be non-intersecting. Vertices after reorder: [{verts[0]}, {verts[1]}, {verts[2]}, {verts[3]}]");
    }

    [Test]
    public void ReorderVerts_TrapezoidalQuad_ProducesNonIntersectingTriangles()
    {
        // Trapezoidal quad
        //  3-----2
        //   \   /
        //    \ /
        //  0---1
        var verts = new List<Vector3>
        {
            new Vector3(0.25f, 0, 0),   // bottom-left
            new Vector3(0.75f, 0, 0),   // bottom-right
            new Vector3(1, 1, 0),       // top-right
            new Vector3(0, 1, 0)        // top-left
        };

        var helper = new TetrahedronTestHelper();
        helper.TestReorderVerts(verts);

        bool nonIntersecting = AreTrianglesNonIntersecting(verts);
        Assert.IsTrue(nonIntersecting,
            $"Triangles should be non-intersecting. Vertices after reorder: [{verts[0]}, {verts[1]}, {verts[2]}, {verts[3]}]");
    }

    [Test]
    public void ReorderVerts_RandomConvexQuad1_ProducesNonIntersectingTriangles()
    {
        // Random convex quad
        var verts = new List<Vector3>
        {
            new Vector3(1.2f, 0.3f, 0),
            new Vector3(2.5f, 0.8f, 0),
            new Vector3(2.1f, 2.1f, 0),
            new Vector3(0.5f, 1.7f, 0)
        };

        var helper = new TetrahedronTestHelper();
        helper.TestReorderVerts(verts);

        bool nonIntersecting = AreTrianglesNonIntersecting(verts);
        Assert.IsTrue(nonIntersecting,
            $"Triangles should be non-intersecting. Vertices after reorder: [{verts[0]}, {verts[1]}, {verts[2]}, {verts[3]}]");
    }

    [Test]
    public void ReorderVerts_NegativeCoordinates_ProducesNonIntersectingTriangles()
    {
        // Quad with negative coordinates
        var verts = new List<Vector3>
        {
            new Vector3(-1, -1, 0),
            new Vector3(1, -1, 0),
            new Vector3(1, 1, 0),
            new Vector3(-1, 1, 0)
        };

        var helper = new TetrahedronTestHelper();
        helper.TestReorderVerts(verts);

        bool nonIntersecting = AreTrianglesNonIntersecting(verts);
        Assert.IsTrue(nonIntersecting,
            $"Triangles should be non-intersecting. Vertices after reorder: [{verts[0]}, {verts[1]}, {verts[2]}, {verts[3]}]");
    }

    [Test]
    public void ReorderVerts_VerySmallQuad_ProducesNonIntersectingTriangles()
    {
        // Very small quad (testing numerical precision)
        var verts = new List<Vector3>
        {
            new Vector3(0, 0, 0),
            new Vector3(0.01f, 0, 0),
            new Vector3(0.01f, 0.01f, 0),
            new Vector3(0, 0.01f, 0)
        };

        var helper = new TetrahedronTestHelper();
        helper.TestReorderVerts(verts);

        bool nonIntersecting = AreTrianglesNonIntersecting(verts);
        Assert.IsTrue(nonIntersecting,
            $"Triangles should be non-intersecting. Vertices after reorder: [{verts[0]}, {verts[1]}, {verts[2]}, {verts[3]}]");
    }

    [Test]
    public void ReorderVerts_LargeQuad_ProducesNonIntersectingTriangles()
    {
        // Very large quad
        var verts = new List<Vector3>
        {
            new Vector3(0, 0, 0),
            new Vector3(1000, 0, 0),
            new Vector3(1000, 1000, 0),
            new Vector3(0, 1000, 0)
        };

        var helper = new TetrahedronTestHelper();
        helper.TestReorderVerts(verts);

        bool nonIntersecting = AreTrianglesNonIntersecting(verts);
        Assert.IsTrue(nonIntersecting,
            $"Triangles should be non-intersecting. Vertices after reorder: [{verts[0]}, {verts[1]}, {verts[2]}, {verts[3]}]");
    }

    [Test]
    public void ReorderVerts_QuadWithNonZeroZ_ProducesNonIntersectingTriangles()
    {
        // Quad in a plane with non-zero Z coordinates (should still work as test ignores Z)
        var verts = new List<Vector3>
        {
            new Vector3(0, 0, 5),
            new Vector3(1, 0, 5),
            new Vector3(1, 1, 5),
            new Vector3(0, 1, 5)
        };

        var helper = new TetrahedronTestHelper();
        helper.TestReorderVerts(verts);

        bool nonIntersecting = AreTrianglesNonIntersecting(verts);
        Assert.IsTrue(nonIntersecting,
            $"Triangles should be non-intersecting. Vertices after reorder: [{verts[0]}, {verts[1]}, {verts[2]}, {verts[3]}]");
    }

    [Test]
    public void ReorderVerts_AlmostCollinearPoints_ProducesNonIntersectingTriangles()
    {
        // Four points that are almost collinear (degenerate quad)
        var verts = new List<Vector3>
        {
            new Vector3(0, 0, 0),
            new Vector3(1, 0.01f, 0),
            new Vector3(2, 0.02f, 0),
            new Vector3(3, 0.01f, 0)
        };

        var helper = new TetrahedronTestHelper();
        helper.TestReorderVerts(verts);

        bool nonIntersecting = AreTrianglesNonIntersecting(verts);
        Assert.IsTrue(nonIntersecting,
            $"Triangles should be non-intersecting. Vertices after reorder: [{verts[0]}, {verts[1]}, {verts[2]}, {verts[3]}]");
    }

    [Test]
    public void ReorderVerts_IrregularQuad_ProducesNonIntersectingTriangles()
    {
        // Highly irregular convex quad
        var verts = new List<Vector3>
        {
            new Vector3(0, 0, 0),
            new Vector3(3, 0.5f, 0),
            new Vector3(2.5f, 2, 0),
            new Vector3(0.5f, 1.5f, 0)
        };

        var helper = new TetrahedronTestHelper();
        helper.TestReorderVerts(verts);

        bool nonIntersecting = AreTrianglesNonIntersecting(verts);
        Assert.IsTrue(nonIntersecting,
            $"Triangles should be non-intersecting. Vertices after reorder: [{verts[0]}, {verts[1]}, {verts[2]}, {verts[3]}]");
    }
}
