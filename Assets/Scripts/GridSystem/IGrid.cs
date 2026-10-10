using UnityEngine;
using System.Collections.Generic;

public interface IGrid
{
    int Width { get; }
    int Height { get; }
    Vector3 GetNodeOrigin(GridIndex gridIndex);
    Vector3 GetNodeCenter(GridIndex gridIndex);
    Node GetNode(Vector3 cellWorldPosition);
    void SetNode(Node node, Vector3 cellWorldPosition);
    List<Node> GetRowAt(int colIndex);
    GridIndex GetGridIndex(Vector3 cellWorldPosition);
}
