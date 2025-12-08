using System;
using Assets.Tetrahedralization;
using Mono.Cecil.Cil;
using Unity.Burst.CompilerServices;
using UnityEngine;

[ExecuteInEditMode]
public class Create1Million : MonoBehaviour
{
    [SerializeField] bool m_Start = false;
    GameObject p;
    MeshRenderer m;

    // Start is called once before the first execution of Update after the MonoBehaviour is created;
    void Start()
    {
        p = new GameObject();
        p.transform.SetParent(gameObject.transform);
        m = p.AddComponent<MeshRenderer>();
    }
    void OnValidate()
    {
        if (!m_Start) return;
        m_Start = false;
        UnityEngine.Random.InitState((int)Time.time);
        for (int i = 0; i < 500; i++)
        {
            var a = new GameObject();
            a.AddComponent<MeshCollider>();
            a.AddComponent<Assets.Tetrahedralization.Old.CreateSimplex>();
            a.AddComponent<MeshFilter>();
            a.AddComponent<MeshRenderer>().sharedMaterial = m.sharedMaterial;
            a.transform.position = new(UnityEngine.Random.Range(0, 10), UnityEngine.Random.Range(0, 10), UnityEngine.Random.Range(0, 10));
            a.transform.SetParent(p.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
