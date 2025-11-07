using System.Collections.Generic;

namespace Script.Graph
{
    public class Graph
    {
        #region IDs
        private static int _nextNodeId = 0;
        private static int _nextEdgeId = 0;
        
        public static int GetNextNodeId()
        {
            return _nextNodeId++;
        }
        
        public static int GetNextEdgeId()
        {
            return _nextEdgeId++;
        }
        #endregion
        
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
        
    }
}