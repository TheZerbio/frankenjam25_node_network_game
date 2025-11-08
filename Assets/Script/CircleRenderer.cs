using UnityEngine;

// Add this component to a GameObject to see the dashed circles
public class DashedCircleDrawer : MonoBehaviour
{
    [System.Serializable]
    public class Circle
    {
        public float radius = 3f;
        public Color color = Color.white;
        
        [Tooltip("The number of dashes the circle will be split into.")]
        [Range(1, 100)] // Use Range for a slider in the inspector
        public int segmentCount = 18; 
    }

    [Header("Circles to draw")]
    public Circle[] circles = new Circle[]
    {
        new Circle { radius = 2f, color = Color.red, segmentCount = 8 },
        new Circle { radius = 4f, color = Color.green, segmentCount = 12 },
        new Circle { radius = 6f, color = Color.blue, segmentCount = 16 }
    };

    [Header("Line Renderer Settings")]
    public float lineWidth = 0.05f;

    private LineRenderer[] lineRenderers;

    void Update()
    {
        KillAllChildren();
        DrawCircles();
    }
    private void Start()
    {
        CreateLineRenderers();
        DrawCircles();
    }

    private void CreateLineRenderers()
    {
        // Destroy old renderers first to prevent duplicates
        if (lineRenderers != null)
        {
            foreach (var lr in lineRenderers)
            {
                if (lr != null)
                {
                    // Use DestroyImmediate when in editor (like OnValidate)
                    if (Application.isPlaying)
                        Destroy(lr.gameObject);
                    else
                        DestroyImmediate(lr.gameObject);
                }
            }
        }

        lineRenderers = new LineRenderer[circles.Length];

        for (int i = 0; i < circles.Length; i++)
        {
            GameObject go = new GameObject("Circle_" + i);
            go.transform.parent = transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;

            LineRenderer lr = go.AddComponent<LineRenderer>();
            lr.useWorldSpace = true; // Points will be in world space
            lr.loop = false; // We are drawing segments, not a continuous loop
            
            // Use a simple default material
            lr.material = new Material(Shader.Find("Sprites/Default")); 
            
            lr.widthMultiplier = lineWidth;
            lr.positionCount = 0; // Start empty

            lineRenderers[i] = lr;
        }
    }

    /// <summary>
    /// Calculates and draws all the dashed circles.
    /// </summary>
    private void DrawCircles()
    {
        if (lineRenderers == null) return;

        for (int i = 0; i < circles.Length; i++)
        {
            if (i >= lineRenderers.Length || lineRenderers[i] == null)
            {
                Debug.LogWarning("LineRenderer missing, recreating...");
                CreateLineRenderers(); // Something went wrong, try to fix
            }
            
            Circle circle = circles[i];
            LineRenderer lr = lineRenderers[i];

            // Apply all settings
            lr.startColor = lr.endColor = circle.color;
            lr.widthMultiplier = lineWidth;
            lr.material.color = circle.color; // Set material tint

            // Ensure we have at least 4 segment
            int segmentCount = Mathf.Max(4, circle.segmentCount);

            // Calculate the angle for one full segment (one dash + one gap)
            float anglePerFullSegment = 360f / segmentCount;
            
            // The dash will be half of that angle
            float dashAngle = anglePerFullSegment / 2f;

            // Each dash needs 2 points (a start and an end)
            int pointCount = segmentCount * 2;
            Vector3[] points = new Vector3[pointCount];

            for (int j = 0; j < segmentCount; j++)
            {
                // Calculate the start and end angles for this dash
                float startAngleDeg = j * anglePerFullSegment;
                float endAngleDeg = startAngleDeg + dashAngle;

                // Convert from degrees to radians for trigonometric functions
                float startAngleRad = Mathf.Deg2Rad * startAngleDeg;
                float endAngleRad = Mathf.Deg2Rad * endAngleDeg;

                // Get the direction vectors
                Vector3 startDir = new Vector3(Mathf.Cos(startAngleRad), Mathf.Sin(startAngleRad), 0f);
                Vector3 endDir = new Vector3(Mathf.Cos(endAngleRad), Mathf.Sin(endAngleRad), 0f);

                // Calculate the world positions for the points
                Vector3 startPos = transform.position + startDir * circle.radius;
                Vector3 endPos = transform.position + endDir * circle.radius;

                // Assign points to the array
                points[j * 2] = startPos;
                points[j * 2 + 1] = endPos;
            }

            // Apply the points to the LineRenderer
            lr.positionCount = pointCount;
            lr.SetPositions(points);
        }
    }

    private void KillAllChildren()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}