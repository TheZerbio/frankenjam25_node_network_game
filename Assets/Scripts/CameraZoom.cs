using TMPro;
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

    [Header("Level Generation")] 
    public Texture2D Background = null;
    public Color lineColor = Color.white;
    public float TextureScaleFactor = 1f;
    
    [Header("Debug")]
    [Tooltip("Draws the camera bounds rectangle in the Scene view.")]
    public Color boundsColor = Color.yellow;

    
    
    private Camera cam;
    private bool isPanning = false;
    private Vector3 panOriginWorld;

    public void Start()
    {
        if (!Background)
            Background = Texture2D.grayTexture;
        GenerateBoundaryColliders();
        CreateTiledBackground(Background,TextureScaleFactor);
    }
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
        HandleKeyMove();
    }

    private void HandleKeyMove()
    {
        var move = InputSystem.actions["Move"].ReadValue<Vector2>();
        if (move.magnitude != 0)
        {
            transform.position = transform.position+(Vector3)move;
            ClampCameraPosition();
        }
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
    
    #region LevelGen
    //Level Bounds
    private void GenerateBoundaryColliders()
    {
        float thickness = 1f; // how thick the boundary walls are
        float minX = minBounds.x;
        float maxX = maxBounds.x;
        float minY = minBounds.y;
        float maxY = maxBounds.y;

        // Create a parent object to keep things organized
        GameObject boundaryParent = new GameObject("CameraBoundaries");
        boundaryParent.transform.SetParent(transform);

        // Bottom
        CreateBoundaryCollider(boundaryParent.transform, new Vector2((minX + maxX) / 2f, minY - thickness / 2f),
            new Vector2(maxX - minX, thickness));

        // Top
        CreateBoundaryCollider(boundaryParent.transform, new Vector2((minX + maxX) / 2f, maxY + thickness / 2f),
            new Vector2(maxX - minX, thickness));

        // Left
        CreateBoundaryCollider(boundaryParent.transform, new Vector2(minX - thickness / 2f, (minY + maxY) / 2f),
            new Vector2(thickness, maxY - minY));

        // Right
        CreateBoundaryCollider(boundaryParent.transform, new Vector2(maxX + thickness / 2f, (minY + maxY) / 2f),
            new Vector2(thickness, maxY - minY));
    }

    private void CreateBoundaryCollider(Transform parent, Vector2 position, Vector2 size)
    {
        GameObject obj = new GameObject("BoundaryCollider");
        obj.transform.SetParent(parent);
        obj.transform.position = position;

        BoxCollider2D col = obj.AddComponent<BoxCollider2D>();
        col.size = size;
        col.isTrigger = false;
    }
    
    // Background
    private void CreateTiledBackground(Texture2D texture, float tileWorldSize = 5f)
    {
        if (texture == null)
        {
            Debug.LogWarning("CreateTiledBackground: Texture is null!");
            return;
        }

        // Ensure texture can tile
        texture.wrapMode = TextureWrapMode.Repeat;

        // Create a material that supports texture tiling
        Material mat = new Material(Shader.Find("Sprites/Default"));
        mat.mainTexture = texture;
        mat.color = lineColor;

        // Create a new GameObject for the background
        GameObject bg = new GameObject("TiledBackground");
        bg.transform.position = new Vector3(
            (minBounds.x + maxBounds.x) / 2f,
            (minBounds.y + maxBounds.y) / 2f,
            10f // behind everything
        );

        // Add SpriteRenderer
        SpriteRenderer sr = bg.AddComponent<SpriteRenderer>();
        sr.sharedMaterial = mat;
        sr.drawMode = SpriteDrawMode.Tiled;

        // Convert texture into sprite
        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            texture.width / tileWorldSize // pixels per unit so tiles appear with chosen world size
        );
        sr.sprite = sprite;

        // Set the size of the tiled area
        sr.size = new Vector2(maxBounds.x - minBounds.x, maxBounds.y - minBounds.y);
    }



    #endregion
}
