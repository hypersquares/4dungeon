using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "Cube4D", menuName = "Cube4D")]
public class Cube4D: Mesh4D
{
    public Cube4D()
	{
        Cube4DInitializer.CreateTesseract(this, -1, 1, -1, 1, -1, 1, -1, 1);
		Debug.Log("Created Tesseract in 4D!");	
    }
}