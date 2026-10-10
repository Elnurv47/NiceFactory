using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    [SerializeField] private GridXY grid;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject buildingPrefab;
    [SerializeField] private KeyCode spawnKey = KeyCode.E;
    [SerializeField] private int footprintSize = 5;

    private GameObject currentCarriedBuilding;
    private bool isCarrying = false;

    private void Awake()
    {
        if (grid == null)
            grid = FindFirstObjectByType<GridXY>();

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update()
    {
        if (grid == null) return;

        if (Input.GetKeyDown(spawnKey) && !isCarrying)
        {
            SpawnBuildingAtMouse();
        }

        if (isCarrying && currentCarriedBuilding != null)
        {
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;

            Node bottomLeftNode = grid.GetNode(mouseWorldPos);
            if (bottomLeftNode != null)
            {
                currentCarriedBuilding.transform.position = CalculateBuildingOrigin(bottomLeftNode.GridIndex);
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (bottomLeftNode != null && CanPlaceBuilding(bottomLeftNode.GridIndex))
                {
                    PlaceBuilding(bottomLeftNode, currentCarriedBuilding);
                }
            }
        }
    }

    private void SpawnBuildingAtMouse()
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        currentCarriedBuilding = Instantiate(buildingPrefab, mouseWorldPos, Quaternion.identity);
        isCarrying = true;
    }

    private bool CanPlaceBuilding(GridIndex bottomLeftIndex)
    {
        List<GridIndex> footprint = GetFootprintIndices(bottomLeftIndex);

        foreach (GridIndex index in footprint)
        {
            Vector3 worldPos = grid.GetNodeCenter(index);
            Node node = grid.GetNode(worldPos);

            if (node == null || !node.IsAvailable)
            {
                return false;
            }
        }

        return true;
    }

    private void PlaceBuilding(Node bottomLeftNode, GameObject building)
    {
        List<GridIndex> footprint = GetFootprintIndices(bottomLeftNode.GridIndex);

        foreach (GridIndex index in footprint)
        {
            Vector3 worldPos = grid.GetNodeCenter(index);
            Node node = grid.GetNode(worldPos);
            if (node != null)
            {
                node.SetObject(building);
            }
        }

        building.transform.position = CalculateBuildingOrigin(bottomLeftNode.GridIndex);
        currentCarriedBuilding = null;
        isCarrying = false;
    }

    public bool SpawnRandomBuilding()
    {
        int maxX = grid.Width - footprintSize;
        int maxY = grid.Height - footprintSize;

        int randomX = Random.Range(0, maxX + 1);
        int randomY = Random.Range(0, maxY + 1);
        GridIndex bottomLeftIndex = new GridIndex(randomX, randomY, 0);

        if (CanPlaceBuilding(bottomLeftIndex))
        {
            Vector3 spawnPos = CalculateBuildingOrigin(bottomLeftIndex);
            GameObject newBuilding = Instantiate(buildingPrefab, spawnPos, Quaternion.identity);

            Node bottomLeftNode = grid.GetNode(grid.GetNodeCenter(bottomLeftIndex));
            PlaceBuilding(bottomLeftNode, newBuilding);
            return true;
        }

        return false;
    }

    private Vector3 CalculateBuildingOrigin(GridIndex bottomLeftIndex)
    {
        Vector3 nodeCenter = grid.GetNodeCenter(bottomLeftIndex);
        float offset = (footprintSize - 1) * grid.CellSize * 0.5f;
        return nodeCenter + new Vector3(offset, offset, 0f);
    }

    private List<GridIndex> GetFootprintIndices(GridIndex bottomLeftIndex)
    {
        List<GridIndex> indices = new List<GridIndex>();

        for (int x = 0; x < footprintSize; x++)
        {
            for (int y = 0; y < footprintSize; y++)
            {
                indices.Add(new GridIndex(bottomLeftIndex.X + x, bottomLeftIndex.Y + y, 0));
            }
        }

        return indices;
    }
}