using System.Linq;
using Script.Graph;
using UnityEngine;

namespace Script
{
    public class GameManger : MonoBehaviour
    {
        #region IDs
        private static int _nextNodeId = 0;
        private static int _nextEdgeId = 0;
        private static int _nextWorkerId = 0;
        
        public static int GetNextNodeId()
        {
            return ++_nextNodeId;
        }
        
        public static int GetNextEdgeId()
        {
            return _nextEdgeId++;
        }
        
        public static int GetNextWorkerId()
        {
            return _nextWorkerId++;
        }
        #endregion
        
        private static GameManger _instance;
        public Color[] colors = new Color[8];
        public Color[] highlightedColors;
        public GameObject nodePrefab;
        public GameObject edgePrefab;
        private Graph.Graph[] PlayerGraphs = new Graph.Graph[2];
        
        public static GameManger GetInstance()
        {
            if (_instance == null)
            {
                Debug.LogError("GameManger not found, Make Sure you have one in our Scene");
                return null;
            }
            return _instance;
        }

        void Start()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Debug.LogError("Multiple GameManger found, Make Sure you have only one in our Scene");
                Destroy(this);
            }
            for (int i = 0; i < PlayerGraphs.Length; i++)
            {
                PlayerGraphs[i] = new Graph.Graph();
            }
        }

        public bool AddNodeToPlayerGraph(Node node, int fraction)
        {
            if (fraction == -1)
                return false;
            return PlayerGraphs[fraction].AddNode(node);
        }
        
        public Node CreateNode(Vector3 position, int fraction)
        {
            var nodeGO = Instantiate(nodePrefab, position, Quaternion.identity);
            Node node = nodeGO.GetComponent<Node>();
            node.fractionID = fraction;
            node.Start();
            PlayerGraphs[fraction].AddNode(node);
            return node;
        }

        public bool CreateEdge(Node start, Node end, int fraction)
        {
            
            if (start.fractionID == end.fractionID || start.fractionID == -1 && end.fractionID != -1 || end.fractionID == -1 && start.fractionID != -1) //Allow connections beween the same faction and Factionless nodes
            {
                var edgeGO = Instantiate(edgePrefab, Vector3.zero, Quaternion.identity);
                StandardEdge edge = edgeGO.GetComponent<StandardEdge>();
                edge.Configure(start, end);
                if (start.fractionID == -1)
                {
                    start.fractionID = end.fractionID;
                    PlayerGraphs[end.fractionID].AddNode(start);
                }

                if (end.fractionID == -1)
                {
                    end.fractionID = start.fractionID;
                    PlayerGraphs[start.fractionID].AddNode(end);
                }
                foreach (Edge e  in start.edges.Union(end.edges))
                {
                    if (e.Equals(edge))
                    {
                        Debug.Log("Edge already exists, skipping");
                    }
                }
                start.edges.Add(edge);
                end.edges.Add(edge);
                return true;
            }
            return false;
            
        }

        public void DestroyEdge(Edge edge)
        {
            if(edge._node1)
                edge._node1.edges.Remove(edge);
            if(edge._node2)
                edge._node2.edges.Remove(edge);
            Destroy(edge.gameObject);
        }
        
        public bool BuildNodeFromNode(Node node, Vector3 position)
        {
            if (Vector2.Distance(node.transform.position,position) > node.workRadius)
                return false;
            
            Node newNode = CreateNode(position, node.fractionID);
            CreateEdge(node, newNode, node.fractionID);
            return true;
        }
        
        
    }
}