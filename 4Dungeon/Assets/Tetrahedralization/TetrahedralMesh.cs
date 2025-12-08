using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
namespace Assets.Tetrahedralization
{
public class TetrahedralMesh
{
    public Vector4[] verts;
    public Tetrahedron[] tetrs;
    public Transform transform;

    public TetrahedralMesh(Vector4[] verts, Tetrahedron[] tetrs)
    {
        this.verts = verts;
        this.tetrs = tetrs;
    }

}

}
