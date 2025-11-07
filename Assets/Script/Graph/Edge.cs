namespace Script.Graph
{
    public abstract class Edge
    {
        public final int _id = Gamemanger.GetNextEdgeId();
        private Node  _node1;
        private Node  _node2;

        private double disconnectionDistance;

        public bool Equals(Edge obj)
        {
            return this._id == obj.GetId();
        }
        
        public int GetId(){return _id;}
    }
}
