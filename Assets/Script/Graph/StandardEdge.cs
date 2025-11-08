using UnityEngine;

namespace Script.Graph
{
    public class StandardEdge : Edge
    {
        private TexturedLine _line;
        public Texture2D lineTexture;

        void Start()
        {
            _line = gameObject.AddComponent<TexturedLine>();
            _line.lineTexture = lineTexture;
            _line.pointA = _node1.transform;
            _line.pointB = _node2.transform;
        }
        
        
    }
}