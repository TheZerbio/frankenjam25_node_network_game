using System.Collections.Generic;
using UnityEngine;

namespace Script.Graph
{
    public class DrawDashedLineClass : MonoBehaviour
    {
        public int DashMultiplier = 1;
        public float lineWidth = 1;
        public Color lineColor = Color.white;
        public List<Command> commands;
        public bool showLine = true;
        
        private Texture2D tex;
        private LineRenderer _lineRenderer;

        void Start()
        {
            tex = GenerateDashTexture(DashMultiplier);
            GameObject lrObj = new GameObject("CircleLR");
            lrObj.transform.parent = transform;
            lrObj.transform.localPosition = Vector3.zero;

            LineRenderer lr = lrObj.AddComponent<LineRenderer>();
            lr.useWorldSpace = true;
            lr.loop = true;
            lr.widthMultiplier = lineWidth;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.sortingLayerName = "Dashes";
            lr.sortingOrder = -1;
            lr.loop = false;
            _lineRenderer = lr;
        }
        
        private void FixedUpdate()
        {
            DrawDashedLine();
        }

        private void DrawDashedLine()
        {
            if (commands is not { Count: > 0 }) return;
            if (_lineRenderer == null) return;

            _lineRenderer.positionCount = commands.Count;

            Vector3[] points = new Vector3[commands.Count];
            var comArray = commands.ToArray();
            
            for (int i = commands.Count - 1; i >= 0; i--)
            {
                var index = (commands.Count - 1) - i;
                points[index]= comArray[i].targetPosition ?? comArray[i].targetObject.getGameObject().transform.position;
            }
            _lineRenderer.SetPositions(points);
            _lineRenderer.startColor = _lineRenderer.endColor = lineColor;
            _lineRenderer.material.color =lineColor;
            _lineRenderer.material.mainTexture = tex;

            // Update visibility
            _lineRenderer.gameObject.SetActive(showLine);
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
    }
}