using UnityEngine;
using Assets.Tetrahedralization;
using UnityEditor;
using UnityEditor.EditorTools;
using Unity.VisualScripting;
using System.Collections;

[ExecuteInEditMode]
public class ManySimplicesDisplayCase : MonoBehaviour
{
    public int numSimplices = 100;

    public Vector4 bounds = new(10, 10, 10, 2);
    public Transform anchorPos;
    public bool DestroyAndRegenerateChildren = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gameObject.transform.childCount == 0) {
            AddSimplices();
        }
    }
    void AddSimplices() {
        for (int i = 0; i < numSimplices; i++) {
            GameObject simplex = new GameObject("Simplex " + i);
            var trans = simplex.AddComponent<Transform4D>();
            trans.Position = (Vector4) anchorPos.position + new Vector4(Random.value * bounds.x, Random.value * bounds.y, Random.value * bounds.z, Random.value * bounds.w);
            trans.Rotation = new Euler4 {
                XY = Random.value * 360,
                YZ = Random.value * 360,
                XZ = Random.value * 360,
                XW = Random.value * 360,
                YW = Random.value * 360,
                ZW = Random.value * 360
            };
            if (i % 3 == 0) {
                trans.Scale = new Vector4(Random.value + 0.6f, Random.value + 0.6f, Random.value + 0.7f, Random.value + 0.5f) * 0.25f;
            } else
            {
                trans.Scale = new Vector4(Random.value * 0.4f, Random.value * 0.4f, Random.value * 0.4f, Random.value + 0.5f);
            }
            simplex.transform.SetParent(gameObject.transform, false);
            var slicer = simplex.AddComponent<TetrahedralMeshSlicer>();
            slicer.m_baseColor = new Color(Random.value, Random.value, Random.value);
            slicer.m_FarColor = new Color(Random.value, Random.value, Random.value);
            simplex.AddComponent<CreateSimplex>();
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(DestroyAndRegenerateChildren)
        {
            DestroyAndRegenerateChildren = false;
            foreach (Transform childTransform in gameObject.transform)
            {
                DestroyImmediate(childTransform.gameObject);
            }
            StartCoroutine("Stnky");
        }
    }

    IEnumerator Stnky()
    {
        yield return null;
        AddSimplices();
    } 
}




