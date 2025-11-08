using System.Collections.Generic;
using UnityEditor;

namespace Script.Graph
{
    public class Graph
    {
        private List<Edge> Edges = new List<Edge>();
        private List<Node> Nodes = new List<Node>();

        public List<Node> GetNodes()
        {
            return Nodes;
        }

        public List<Edge> GetEdges()
        {
            return Edges;
        }
        
        public void AddEdge(Edge edge)
        {
            Edges.Add(edge);
        }

        public void AddNode(Node node)
        {
            Nodes.Add(node);
        }

        public Node RemoveNode(Node node)
        {
            Nodes.Remove(node);
            return node;
        }
        
        /// <summary>
        /// Return null if NodeID not in Graph
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        public Node RemoveNodeById(int nodeID)
        {
            var node = GetNodeById(nodeID);
            if (node)
                Nodes.Remove(node);
            return node;
        }

        /// <summary>
        /// Returns the node if in Graph, else returns null
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        public Node GetNodeById(int nodeID)
        {
            foreach (Node node in Nodes)
            {
                if (node.id ==  nodeID)
                    return node;
            }
            return null;
        }
    }
}