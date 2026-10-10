using Utils;
using UnityEngine;
using System.Collections.Generic;

public class GridXY : MonoBehaviour, IGrid
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize;
    [SerializeField] private Vector3 origin;

    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private Transform nodeContainer;

    [SerializeField] private bool debug;

    private Node[,] gridArray;
    private TextMesh[,] gridDebugArray;

    public int Width { get => width; }
    public int Height { get => height; }
    public float CellSize { get => cellSize; }

    private void Awake()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        gridArray = new Node[width, height];
        gridDebugArray = new TextMesh[width, height];

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                GridIndex gridIndex = new GridIndex(x, y, 0);

                GameObject spawnedNodeObject = Instantiate(nodePrefab, GetNodeCenter(gridIndex), Quaternion.identity, nodeContainer);

                Node spawnedNode = spawnedNodeObject.GetComponent<Node>();
                SetNode(spawnedNode, gridIndex);

                #region Debugging

                if (debug)
                {
                    gridDebugArray[x, y] = Utility.CreateWorldText(
                        gridArray[x, y].ToString(),
                        position: GetNodeOrigin(gridIndex) + GetOffsetBetweenNodeOriginAndCenterMath(),
                        rotation: Quaternion.identity,
                        localScale: new Vector3(0.15f, 0.15f, 0.15f),
                        color: Color.white,
                        fontSize: 25
                    );

                    Debug.DrawLine(GetNodeOriginDebug(new GridIndex(x, y, 0)), GetNodeOriginDebug(new GridIndex(x, y + 1, 0)), Color.white, 1000f);
                    Debug.DrawLine(GetNodeOriginDebug(new GridIndex(x, y, 0)), GetNodeOriginDebug(new GridIndex(x + 1, y, 0)), Color.white, 1000f);
                }

                #endregion
            }
        }

        #region Debugging

        if (debug)
        {
            Debug.DrawLine(GetNodeOriginDebug(new GridIndex(0, height, 0)), GetNodeOriginDebug(new GridIndex(width, height, 0)), Color.white, 1000f);
            Debug.DrawLine(GetNodeOriginDebug(new GridIndex(width, 0, 0)), GetNodeOriginDebug(new GridIndex(width, height, 0)), Color.white, 1000f);
        }

        #endregion
    }

    private Vector3 GetNodeOriginDebug(GridIndex gridIndex)
    {
        return gridIndex.ToVector3() * cellSize + origin;
    }

    public Vector3 GetNodeOrigin(Node node)
    {
        return GetNodeOrigin(node.GridIndex);
    }

    public Vector3 GetNodeOrigin(Vector3 nodePosition)
    {
        GridIndex gridIndex = GetGridIndex(nodePosition);
        return GetNodeOrigin(gridIndex);
    }

    public Vector3 GetNodeOrigin(GridIndex gridIndex)
    {
        if (!IsValidCoordinate(gridIndex))
        {
            GridIndex clampedGridIndex = ClampToGrid(gridIndex);
            return GetNodeOrigin(clampedGridIndex);
        }

        return gridIndex.ToVector3() * cellSize + origin;
    }

    private GridIndex ClampToGrid(GridIndex gridIndex)
    {
        gridIndex.X = Mathf.Clamp(gridIndex.X, 0, width - 1);
        gridIndex.Y = Mathf.Clamp(gridIndex.Y, 0, height - 1);

        return gridIndex;
    }

    public Vector3 GetNodeCenter(GridIndex gridIndex)
    {
        if (!IsValidCoordinate(gridIndex))
        {
            gridIndex = ClampToGrid(gridIndex);
        }

        Vector3 nodeOrigin = GetNodeOrigin(gridIndex);
        Vector3 offsetBetweenNodeOriginAndCenter = GetOffsetBetweenNodeOriginAndCenterMath();
        Vector3 nodeCenter = nodeOrigin + offsetBetweenNodeOriginAndCenter;

        return nodeCenter;
    }

    public Vector3 GetNodeCenter(Node node)
    {
        return GetNodeCenter(node.GridIndex);
    }

    public Vector3 GetNodeCenter(Vector3 nodePosition)
    {
        GridIndex gridIndex = GetGridIndex(nodePosition);
        return GetNodeCenter(gridIndex);
    }

    private Vector3 GetOffsetBetweenNodeOriginAndCenterMath()
    {
        return new Vector3(cellSize / 2f, cellSize / 2f, 0f);
    }

    public void SetNode(Node node, Vector3 nodePosition)
    {
        GridIndex gridIndexAtPosition = GetGridIndex(nodePosition);
        SetNode(node, gridIndexAtPosition);
    }

    private void SetNode(Node node, GridIndex gridIndex)
    {
        if (!IsValidCoordinate(gridIndex)) return;

        node.Initialize(this, gridIndex);

        gridArray[gridIndex.X, gridIndex.Y] = node;
    }

    public GridIndex GetGridIndex(Vector3 nodeWorldPosition)
    {
        int x = Mathf.FloorToInt((nodeWorldPosition - origin).x / cellSize);
        int y = Mathf.FloorToInt((nodeWorldPosition - origin).y / cellSize);

        GridIndex gridIndex = new GridIndex(x, y, 0);

        return IsValidCoordinate(gridIndex) ? gridIndex : ClampToGrid(gridIndex);
    }

    public Node GetNode(Vector3 cellWorldPosition)
    {
        GridIndex gridIndex = GetGridIndex(cellWorldPosition);
        return IsValidCoordinate(gridIndex) ? gridArray[gridIndex.X, gridIndex.Y] : default;
    }

    private bool IsValidCoordinate(GridIndex gridIndex)
    {
        return gridIndex.X >= 0 && gridIndex.Y >= 0 && gridIndex.X < width && gridIndex.Y < height;
    }

    public List<Node> GetRowAt(int y)
    {
        List<Node> rowNodes = new List<Node>();

        for (int x = 0; x < width; x++)
        {
            Node node = gridArray[x, y];
            rowNodes.Add(node);
        }

        return rowNodes;
    }

    public List<Node> GetColAt(int x)
    {
        List<Node> columnNodes = new List<Node>();

        for (int y = 0; y < height; y++)
        {
            Node node = gridArray[x, y];
            columnNodes.Add(node);
        }

        return columnNodes;
    }

    public Node GetNeighbourNode(Vector3 position, Vector3 direction)
    {
        Vector3 neighbourNodePosition = position + (direction * cellSize);
        return GetNode(neighbourNodePosition);
    }

    public List<Node> GetNeighbours(Node node, bool includeCorners)
    {
        List<Node> neighbours = new List<Node>();

        int x = node.GridIndex.X;
        int y = node.GridIndex.Y;

        int[] dx = { 0, 1, 0, -1 };
        int[] dy = { 1, 0, -1, 0 };

        for (int i = 0; i < 4; i++)
        {
            int nx = x + dx[i];
            int ny = y + dy[i];

            if (IsValidCoordinate(new GridIndex(nx, ny, 0)))
            {
                neighbours.Add(gridArray[nx, ny]);
            }
        }

        if (includeCorners)
        {
            int[] cdx = { 1, 1, -1, -1 };
            int[] cdy = { 1, -1, -1, 1 };

            for (int i = 0; i < 4; i++)
            {
                int nx = x + cdx[i];
                int ny = y + cdy[i];

                if (IsValidCoordinate(new GridIndex(nx, ny, 0)))
                {
                    neighbours.Add(gridArray[nx, ny]);
                }
            }
        }

        return neighbours;
    }
}