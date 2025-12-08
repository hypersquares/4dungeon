using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using System;
using Mono.Cecil;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Burst.Intrinsics;

namespace Assets.Tetrahedralization
{
public struct Tetrahedron {
    public int v0, v1, v2, v3;
    public Tetrahedron(int v0, int v1, int v2, int v3)
    {
        (this.v0, this.v1, this.v2, this.v3) = (v0, v1, v2, v3);
    }
}
}
