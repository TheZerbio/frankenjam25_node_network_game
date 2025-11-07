using UnityEngine;

namespace Script.Graph
{
    public abstract class Edge : MonoBehaviour
    {
        private int _id { get; } = GameManger.GetNextEdgeId();
        private Node  _node1;
        private Node  _node2;

        private double _disconnectionDistance {get;} = -1;

        public bool Equals(Edge obj)
        {
            return this._id == obj.GetId();
        }
        
        public int GetId(){return _id;}
    }
}
