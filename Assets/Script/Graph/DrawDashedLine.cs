using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Script.Graph
{
    public class DrawDashedLineClass : MonoBehaviour
    {
        public int DashMultiplier = 1;
        public float lineWidth = 1;
        public Color lineColor = Color.white;
        public Worker worker;
        
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
            if (_lineRenderer == null || worker == null) return;
            if (!worker.isSelected)
            {
                _lineRenderer.gameObject.SetActive(false);
                return;
            }
            
            List<Command> commands = worker.NextCommands;
            if (commands is not { Count: > 0 }) return;
            while (commands[0].IsWorker() 
                   && commands[0].getWorker().fractionID == worker.fractionID 
                   && commands[0].getWorker().NextCommands.Count > 0
                   && commands.Count < 16)
            {
                commands.InsertRange(0, commands[0].getWorker().NextCommands);
            }
            if (_lineRenderer == null) return;

            _lineRenderer.positionCount = commands.Count + 1;

            Vector3[] points = new Vector3[commands.Count + 1];
            var comArray = commands.ToArray();
            
            points[0] = worker.transform.position;

            for (int i = 1; i <= commands.Count; i++)
            {
                points[i]= comArray[^i].GetTargetPosition();
            }
            
            _lineRenderer.SetPositions(points);
            _lineRenderer.startColor = _lineRenderer.endColor = lineColor;
            _lineRenderer.material.color =lineColor;
            _lineRenderer.material.mainTexture = tex;

            // Update visibility
            _lineRenderer.gameObject.SetActive(worker.isSelected);
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