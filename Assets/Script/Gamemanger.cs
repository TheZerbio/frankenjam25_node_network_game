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
            return _nextNodeId++;
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
        public Color[] colors;
        public GameObject nodePrefab;
        public GameObject edgePrefab;
        public Graph.Graph[] PlayerGraphs = new Graph.Graph[2];
        
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

        public Node CreateNode(Vector3 position, int fraction)
        {
            var nodeGO = Instantiate(nodePrefab, position, Quaternion.identity);
            var node = nodeGO.GetComponent<Node>();
            PlayerGraphs[fraction].AddNode(node);
            return node;
        }

        public void CreateEdge(Node start, Node end, int fraction)
        {
            
            if (PlayerGraphs[fraction].GetNodes().Contains(end) && PlayerGraphs[fraction].GetNodes().Contains(start))
            {
                var edgeGO = Instantiate(nodePrefab, Vector3.zero, Quaternion.identity);
                var edge = edgeGO.GetComponentInChildren<StandardEdge>();
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

        
        public bool BuildNodeFromNode(Node node, Vector3 position, int fraction)
        {
            if ((node.transform.position - position).magnitude <= node.workRadius)
                return false;
            var newNode = CreateNode(position, fraction);
            CreateEdge(node, newNode, fraction);
            return true;
        }
        
        
    }
}