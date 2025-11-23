using System;


[Serializable]
public struct Edge
{
	public int Index0;
	public int Index1;
	public Edge(int index0, int index1)
	{
		Index0 = index0;
		Index1 = index1;
	}

    public override string ToString()
    {
        return $"Edge({Index0}, {Index1})";
    }
}