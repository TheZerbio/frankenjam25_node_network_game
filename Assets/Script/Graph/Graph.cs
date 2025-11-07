using System.Collections.Generic;

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
        
    }
}