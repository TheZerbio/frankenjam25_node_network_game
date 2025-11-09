using System;
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

        public void CreateEdge(Node start, Node end, int fraction)
        {
            
            if (PlayerGraphs[fraction].GetNodes().Contains(end) && PlayerGraphs[fraction].GetNodes().Contains(start))
            {
                var edgeGO = Instantiate(edgePrefab, Vector3.zero, Quaternion.identity);
                StandardEdge edge = edgeGO.GetComponent<StandardEdge>();
                edge.Configure(start, end);
                foreach (Edge e  in PlayerGraphs[fraction].GetEdges())
                {
                    if (e.Equals(edge))
                    {
                        Debug.Log("Edge already exists, skipping");
                    }
                }
                PlayerGraphs[fraction].AddEdge(edge);
            }
            
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