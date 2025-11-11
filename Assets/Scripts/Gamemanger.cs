using System;
using System.Linq;
using Script.Graph;
using Units.Graph;
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
        public int CurrentPlayer = 0 ;
        
        public static GameManger GetInstance()
        {
            if (_instance == null)
            {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/GameManager");
                if (prefab != null)
                {
                    var obj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                    _instance = obj.GetComponent<GameManger>();
                }
                else
                {
                    Debug.LogError("GameManager prefab not found!");
                }
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
        }
        
        public Node CreateNode(Vector3 position, int fraction)
        {
            GameObject root = GameObject.Find("DynamicNodes");
            if (root == null)
            {
                root = new GameObject("DynamicNodes");
            }
            var nodeGO = Instantiate(nodePrefab, position, Quaternion.identity);
            nodeGO.transform.SetParent(root.transform, worldPositionStays: true);
            Node node = nodeGO.GetComponent<Node>();
            node.fractionID = fraction;
            node.Start();
            return node;
        }

        public bool CreateEdge(Node start, Node end, int fraction)
        {
            
            if (start.fractionID == end.fractionID || start.fractionID == -1 && end.fractionID != -1 || end.fractionID == -1 && start.fractionID != -1) //Allow connections beween the same faction and Factionless nodes
            {
                var edgeGO = Instantiate(edgePrefab, Vector3.zero, Quaternion.identity);
                StandardEdge edge = edgeGO.GetComponent<StandardEdge>();
                edge.Configure(start, end);
                foreach (Edge e  in start.edges.Union(end.edges))
                {
                    if (e.Equals(edge))
                    {
                        Debug.Log("Edge already exists, skipping");
                    }
                }
                start.edges.Add(edge);
                end.edges.Add(edge);
                Debug.Log("Edge created between "+start.gameObject.name+" and "+end.gameObject.name);
                return true;
            }
            return false;
            
        }

        public void DestroyEdge(Edge edge)
        {
            if (edge)
            {
                if(edge._node1)
                    edge._node1.edges.Remove(edge);
                if(edge._node2)
                    edge._node2.edges.Remove(edge);
                Destroy(edge.gameObject);
            }
        }
        
        public bool BuildNodeFromNode(Node node, Vector3 position)
        {
            if (Vector2.Distance(node.transform.position,position) > node.buildRadius)
                return false;
            
            Node newNode = CreateNode(position, node.fractionID);
            CreateEdge(node, newNode, node.fractionID);
            return true;
        }
        
        
    }
}