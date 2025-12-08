namespace Assets.Tetrahedralization
{
public struct Edge
{
    public int Index0;
    public int Index1;


    public Edge(int start, int end)
    {
        Index0 = start;
        Index1 = end;
    }
}
}