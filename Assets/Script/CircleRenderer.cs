using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Transform))]
public class MultiDashedCircles : MonoBehaviour
{
    [System.Serializable]
    public class Circle
    {
        public float radius = 1f;
        public Color color = Color.white;
        [Tooltip("Multiplier to scale the dash texture. Higher = more dashes.")]
        public int dashCountMultiplier = 1;
        [Tooltip("Number of segments for smoothness.")]
        public int segmentCount = 64;

        [HideInInspector] public LineRenderer lineRenderer;
    }

    public float lineWidth = 0.1f;
    public bool showCircles = true; // NEW: Toggle visibility
    public List<Circle> circles = new List<Circle>();

    private void Awake()
    {
        // Create LineRenderers for each circle
        foreach (var circle in circles)
        {
            if (circle.lineRenderer == null)
            {
                GameObject lrObj = new GameObject("CircleLR");
                lrObj.transform.parent = transform;
                lrObj.transform.localPosition = Vector3.zero;

                LineRenderer lr = lrObj.AddComponent<LineRenderer>();
                lr.useWorldSpace = true;
                lr.loop = true;
                lr.widthMultiplier = lineWidth;
                lr.material = new Material(Shader.Find("Sprites/Default"));

                circle.lineRenderer = lr;
            }
        }

        UpdateVisibility();
    }

    private void FixedUpdate()
    {
        DrawCircles();
    }

    private void DrawCircles()
    {
        foreach (var circle in circles)
        {
            LineRenderer lr = circle.lineRenderer;
            if (lr == null) continue;

            lr.positionCount = circle.segmentCount;

            Vector3[] points = new Vector3[circle.segmentCount];
            float angleStep = 360f / circle.segmentCount;

            for (int i = 0; i < circle.segmentCount; i++)
            {
                float angleRad = Mathf.Deg2Rad * (i * angleStep);
                points[i] = transform.position + new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0f) * circle.radius;
            }

            lr.SetPositions(points);
            lr.startColor = lr.endColor = circle.color;
            lr.material.color = circle.color;
            lr.material.mainTexture = GenerateDashTexture(circle.dashCountMultiplier);
            lr.material.mainTextureScale = new Vector2(circle.segmentCount / 4f * circle.dashCountMultiplier, 1);

            // Update visibility
            lr.gameObject.SetActive(showCircles);
        }
    }

    private Texture2D GenerateDashTexture(int dashCountMultiplier)
    {
        int texWidth = 4 * Mathf.Max(1, dashCountMultiplier);
        int texHeight = 1;
        Texture2D tex = new Texture2D(texWidth, texHeight);
        tex.filterMode = FilterMode.Point;

        Color[] colors = new Color[texWidth * texHeight];
        for (int i = 0; i < texWidth; i++)
        {
            colors[i] = (i % 2 == 0) ? Color.white : new Color(0, 0, 0, 0);
        }

        tex.SetPixels(colors);
        tex.Apply();
        return tex;
    }

    // Call this to toggle circles at runtime
    public void SetCirclesVisible(bool visible)
    {
        showCircles = visible;
        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        foreach (var circle in circles)
        {
            if (circle.lineRenderer != null)
                circle.lineRenderer.gameObject.SetActive(showCircles);
        }
    }
}
