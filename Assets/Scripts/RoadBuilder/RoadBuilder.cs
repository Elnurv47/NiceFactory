using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class RoadBuilder : MonoBehaviour
{
    [SerializeField] private GridXY grid;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject roadPrefab;
    [SerializeField] private GameObject previewPrefab;
    [SerializeField] private int mouseButton = 0;

    private Node startNode;
    private List<GameObject> previewInstances = new List<GameObject>();
    private Dictionary<GridIndex, GameObject> placedRoads = new Dictionary<GridIndex, GameObject>();

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update()
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        Node currentHoverNode = grid.GetNode(mouseWorldPos);

        if (Input.GetMouseButtonDown(mouseButton))
        {
            if (currentHoverNode != null)
            {
                startNode = currentHoverNode;
            }
        }

        if (Input.GetMouseButton(mouseButton) && startNode != null)
        {
            if (currentHoverNode != null)
            {
                List<Node> roadPath = CalculateOrthogonalPath(startNode, currentHoverNode);
                UpdatePreview(roadPath);
            }
        }

        if (Input.GetMouseButtonUp(mouseButton) && startNode != null)
        {
            if (currentHoverNode != null)
            {
                List<Node> roadPath = CalculateOrthogonalPath(startNode, currentHoverNode);
                BuildRoad(roadPath);
            }

            ClearPreview();
            startNode = null;
        }
    }

    private List<Node> CalculateOrthogonalPath(Node start, Node end)
    {
        List<Node> path = new List<Node>();

        int startX = start.GridIndex.X;
        int startY = start.GridIndex.Y;
        int endX = end.GridIndex.X;
        int endY = end.GridIndex.Y;

        int stepY = startY <= endY ? 1 : -1;
        for (int y = startY; y != endY + stepY; y += stepY)
        {
            Vector3 worldPos = grid.GetNodeCenter(new GridIndex(startX, y, 0));
            Node node = grid.GetNode(worldPos);
            if (node != null && !path.Contains(node))
            {
                path.Add(node);
            }
        }

        int stepX = startX <= endX ? 1 : -1;
        for (int x = startX; x != endX + stepX; x += stepX)
        {
            Vector3 worldPos = grid.GetNodeCenter(new GridIndex(x, endY, 0));
            Node node = grid.GetNode(worldPos);
            if (node != null && !path.Contains(node))
            {
                path.Add(node);
            }
        }

        return path;
    }

    private void UpdatePreview(List<Node> path)
    {
        ClearPreview();

        foreach (Node node in path)
        {
            if (placedRoads.ContainsKey(node.GridIndex)) continue;

            Vector3 spawnPos = grid.GetNodeCenter(node);
            GameObject preview = Instantiate(previewPrefab, spawnPos, Quaternion.identity);
            previewInstances.Add(preview);
        }
    }

    private void ClearPreview()
    {
        foreach (GameObject preview in previewInstances)
        {
            Destroy(preview);
        }
        previewInstances.Clear();
    }

    private void BuildRoad(List<Node> path)
    {
        foreach (Node node in path)
        {
            if (placedRoads.ContainsKey(node.GridIndex)) continue;

            Vector3 spawnPos = grid.GetNodeCenter(node);
            GameObject road = Instantiate(roadPrefab, spawnPos, Quaternion.identity);
            placedRoads.Add(node.GridIndex, road);
        }
    }
}