using UnityEngine;

public class Tesseract
{
    public Vector4[] verts = new Vector4[16]
    {
        new Vector4(-0.5f, -0.5f, -0.5f, -0.5f),
        new Vector4(-0.5f, -0.5f, -0.5f, 0.5f),
        new Vector4(-0.5f, -0.5f, 0.5f, -0.5f),
        new Vector4(-0.5f, -0.5f, 0.5f, 0.5f),
        new Vector4(-0.5f, 0.5f, -0.5f, -0.5f),
        new Vector4(-0.5f, 0.5f, -0.5f, 0.5f),
        new Vector4(-0.5f, 0.5f, 0.5f, -0.5f),
        new Vector4(-0.5f, 0.5f, 0.5f, 0.5f),
        new Vector4(0.5f, -0.5f, -0.5f, -0.5f),
        new Vector4(0.5f, -0.5f, -0.5f, 0.5f),
        new Vector4(0.5f, -0.5f, 0.5f, -0.5f),
        new Vector4(0.5f, -0.5f, 0.5f, 0.5f),
        new Vector4(0.5f, 0.5f, -0.5f, -0.5f),
        new Vector4(0.5f, 0.5f, -0.5f, 0.5f),
        new Vector4(0.5f, 0.5f, 0.5f, -0.5f),
        new Vector4(0.5f, 0.5f, 0.5f, 0.5f)
    };
}