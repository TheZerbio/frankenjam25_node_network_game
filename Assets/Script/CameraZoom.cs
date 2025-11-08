using UnityEngine;
using UnityEngine.InputSystem; // New Input System

[RequireComponent(typeof(Camera))]
public class CameraGrabPanWithBoundsGizmo : MonoBehaviour
{
    [Header("Zoom Settings")]
    [Tooltip("How fast the camera zooms in/out.")]
    public float zoomSpeed = 2f;

    [Tooltip("Minimum orthographic size (max zoom in).")]
    public float minZoom = 2f;

    [Tooltip("Maximum orthographic size (max zoom out).")]
    public float maxZoom = 10f;

    [Header("Pan Settings")]
    [Tooltip("How fast the camera pans when grabbing with the middle mouse. 1 = \"Grab and Pan\"")]
    public float panSpeed = 1f;

    [Header("Camera Bounds (world units)")]
    [Tooltip("Minimum world X and Y position the camera center can reach.")]
    public Vector2 minBounds = new Vector2(-20f, -20f);

    [Tooltip("Maximum world X and Y position the camera center can reach.")]
    public Vector2 maxBounds = new Vector2(20f, 20f);

    [Header("Debug")]
    [Tooltip("Draws the camera bounds rectangle in the Scene view.")]
    public Color boundsColor = Color.yellow;

    private Camera cam;
    private bool isPanning = false;
    private Vector3 panOriginWorld;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        if (!cam.orthographic)
        {
            Debug.LogWarning("CameraGrabPanWithBoundsGizmo is intended for orthographic cameras.");
        }
    }

    private void Update()
    {
        HandleZoom();
        HandleGrabPan();
    }

    private void HandleZoom()
    {
        float scrollValue = Mouse.current.scroll.ReadValue().y;

        if (Mathf.Abs(scrollValue) > 0.01f)
        {
            float zoomChange = -scrollValue * zoomSpeed * Time.deltaTime;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + zoomChange, minZoom, maxZoom);
            ClampCameraPosition();
        }
    }

    private void HandleGrabPan()
    {
        var mouse = Mouse.current;

        if (mouse.middleButton.wasPressedThisFrame)
        {
            isPanning = true;
            panOriginWorld = cam.ScreenToWorldPoint(mouse.position.ReadValue());
        }
        if (mouse.middleButton.wasReleasedThisFrame)
        {
            isPanning = false;
        }

        if (isPanning)
        {
            Vector3 currentWorldPos = cam.ScreenToWorldPoint(mouse.position.ReadValue());
            Vector3 delta = panOriginWorld - currentWorldPos;

            transform.position += delta * panSpeed;
            ClampCameraPosition();
        }
    }

    private void ClampCameraPosition()
    {
        float vertExtent = cam.orthographicSize;
        float horzExtent = vertExtent * cam.aspect;

        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, minBounds.x + horzExtent, maxBounds.x - horzExtent);
        pos.y = Mathf.Clamp(pos.y, minBounds.y + vertExtent, maxBounds.y - vertExtent);

        transform.position = pos;
    }

    private void OnDrawGizmos()
    {
        // Draw bounds rectangle in Scene view
        Gizmos.color = boundsColor;

        Vector3 bottomLeft = new Vector3(minBounds.x, minBounds.y, 0f);
        Vector3 bottomRight = new Vector3(maxBounds.x, minBounds.y, 0f);
        Vector3 topRight = new Vector3(maxBounds.x, maxBounds.y, 0f);
        Vector3 topLeft = new Vector3(minBounds.x, maxBounds.y, 0f);

        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
    }
}
