using System;
using UnityEngine;

[Serializable]
public struct Euler4
{
    [Range(-180, +180)]
    public float XY; // Z (W)
    [Range(-180, +180)]
    public float YZ; // X (w)
    [Range(-180, +180)]
    public float XZ; // Y (W)
    [Range(-180, +180)]
    public float XW; // Y Z
    [Range(-180, +180)]
    public float YW; // X Z
    [Range(-180, +180)]
    public float ZW; // X Y
}