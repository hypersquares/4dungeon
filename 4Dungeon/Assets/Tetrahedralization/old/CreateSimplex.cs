using UnityEngine;

namespace Assets.Tetrahedralization.Old
{
 
[ExecuteInEditMode]
public class CreateSimplex : CreatePolytope
{
    public override TetrahedralMesh GetMesh()
    {
        return new(Simplex.GetTetrahedra(), m_Transform4D);
    }
   
}

}
