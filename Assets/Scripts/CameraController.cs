using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 lastMousePosition;

    [SerializeField] private Camera cam;

    private void Start()
    {
        if (cam == null)
            cam = GetComponent<Camera>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 delta = cam.ScreenToWorldPoint(lastMousePosition) - cam.ScreenToWorldPoint(Input.mousePosition);
            transform.position += delta;
            lastMousePosition = Input.mousePosition;
        }
    }
}