using UnityEngine;

public struct GridIndex
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }

    public GridIndex(int x = 0, int y = 0, int z = 0)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(X, Y, Z);
    }

    public override string ToString()
    {
        return $"{X}:{Y}:{Z}";
    }
}
