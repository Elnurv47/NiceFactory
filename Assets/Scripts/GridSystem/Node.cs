using System;
using UnityEngine;

public class Node : MonoBehaviour
{
    private IGrid grid;

    private GridIndex gridIndex;
    public GridIndex GridIndex { get => gridIndex; }

    private GameObject occupyingObject;

    public void Initialize(IGrid grid, GridIndex gridIndex)
    {
        this.grid = grid;
        this.gridIndex = gridIndex;
    }

    public bool IsAvailable { get => occupyingObject == null; }

    public override string ToString()
    {
        return "X: " + gridIndex.X + ", " + "Z: " + gridIndex.Z;
    }

    public void SetObject(GameObject obj)
    {
        occupyingObject = obj;
    }

    public GameObject GetObject()
    {
        return occupyingObject;
    }
}