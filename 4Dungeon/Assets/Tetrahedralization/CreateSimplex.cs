using UnityEngine;

[ExecuteInEditMode]
public class CreateSimplex : MonoBehaviour
{
    [SerializeField] private Mesh m;
    [SerializeField] private Plane4D plane;
    private MeshFilter m_Filter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        TetrahedralMesh t = new(Simplex.GetTetrahedra());
        m_Filter = gameObject.GetComponent<MeshFilter>();
        m = t.Slice(plane);
        m_Filter.mesh = m;
    }
}
