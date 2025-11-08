using UnityEngine;
using UnityEngine.UIElements;

namespace Script.Graph
{
    public class TexturedLine : MonoBehaviour
    {
        public Transform pointA;
        public Transform pointB;
        public float Volume = 1;
        public Color LineColor = Color.black;
        public Texture2D lineTexture; // assign in the Inspector

        private LineRenderer lr;

        void Start()
        {
            lr = gameObject.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.startWidth = 1;
            lr.endWidth = 1;

            // Create a simple material with a shader that supports textures
            Material mat = new Material(Shader.Find("Sprites/Default"));
            mat.mainTexture = lineTexture;
            mat.mainTextureScale = new Vector2(1, 1); // Repeat scale
            mat.color = LineColor;
            lr.material = mat;

            // Optional: make it tile along the length of the line
            lr.textureMode = LineTextureMode.Tile;
            lr.sortingLayerName = "Bottom";
        }

        void Update()
        {
            if (pointA && pointB)
            {
                var length=  Vector3.Distance(pointA.position, pointB.position);
                lr.startWidth = Volume/length;
                lr.endWidth = Volume/length;
                lr.SetPosition(0, pointA.position);
                lr.SetPosition(1, pointB.position);
            }
        }
    }
}