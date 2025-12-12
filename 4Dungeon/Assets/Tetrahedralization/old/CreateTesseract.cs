using UnityEngine;

namespace Assets.Tetrahedralization.Old
{
 
[ExecuteInEditMode]
public class CreateTesseract : CreatePolytope
{
    public override TetrahedralMesh GetMesh()
    {
        return new(Tesseract.GetTetrahedra(), m_Transform4D);
    }
   
}

}
