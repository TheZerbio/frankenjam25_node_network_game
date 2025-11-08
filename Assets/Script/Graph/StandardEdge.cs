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
            if (!(_node1 && _node2))
            {
                Debug.LogError("Edge not connected to two nodes");
                return;
            }
            MoveBetweenNodes();
            //Draw Visual
            _line.pointA = _node1.gameObject.transform;
            _line.pointB = _node2.gameObject.transform;
            //Connect Springs
            var joint1 = gameObject.AddComponent<SpringJoint2D>();
            joint1.connectedBody = _node1.gameObject.GetComponentInChildren<Rigidbody2D>();
            joint1.autoConfigureDistance = false;
            joint1.distance = (_node2.transform.position - transform.position).magnitude;
            var joint2 =  gameObject.AddComponent<SpringJoint2D>();
            joint2.connectedBody = _node2.gameObject.GetComponentInChildren<Rigidbody2D>();
            joint2.autoConfigureDistance = false;
            joint2.distance = (_node2.transform.position - transform.position).magnitude;
            
        }

        void Update()
        {
            MoveBetweenNodes();
            
        }

        private void MoveBetweenNodes()
        {
            var newposition = _node1.gameObject.transform.position+(_node2.gameObject.transform.position-_node1.gameObject.transform.position)/2;
            transform.position = newposition;
        }

        public void Configure(Node start, Node end)
        {
            _node1 = start;
            _node2 = end;
            MoveBetweenNodes();
        }
        
        
        
    }
}