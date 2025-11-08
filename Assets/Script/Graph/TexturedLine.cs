using UnityEngine;

namespace Script.Graph
{
    public class TexturedLine : MonoBehaviour
    {
        public Transform pointA;
        public Transform pointB;
        public Texture2D lineTexture; // assign in the Inspector

        private LineRenderer lr;

        void Start()
        {
            lr = gameObject.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;

            // Create a simple material with a shader that supports textures
            Material mat = new Material(Shader.Find("Sprites/Default"));
            mat.mainTexture = lineTexture;
            mat.mainTextureScale = new Vector2(1, 1); // Repeat scale
            lr.material = mat;

            // Optional: make it tile along the length of the line
            lr.textureMode = LineTextureMode.Tile;
        }

        void Update()
        {
            if (pointA && pointB)
            {
                lr.SetPosition(0, pointA.position);
                lr.SetPosition(1, pointB.position);
            }
        }
    }
}