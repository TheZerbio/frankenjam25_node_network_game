using System;
using UnityEngine;

namespace Script.Graph
{
    public abstract class Edge : MonoBehaviour
    {
        public static double MaxStrength;
        private int _id { get; } = GameManger.GetNextEdgeId();
        public Node  _node1;
        public Node  _node2;

        private double _disconnectionDistance {get;} = -1;

        public bool Equals(Edge obj)
        {
            return (_node1 == obj._node1 && _node2 == obj._node2) || (_node1 == obj._node2 && _node2 == obj._node1);
        }

        public Edge()
        {
            _id = GameManger.GetNextEdgeId();
            _node1 = null;
            _node2 = null;
            _disconnectionDistance = -1;
        }
        
        public Edge(Node _node1, Node _node2)
        {
            _id = GameManger.GetNextEdgeId();
            this._node1 = _node1;
            this._node2 = _node2;
            _disconnectionDistance = Math.Min(this._node1.workRadius, this._node2.workRadius);
        }

        public bool CheckDissconect()
        {
            return (_node1.position - _node2.position).sqrMagnitude >= _disconnectionDistance;
        }
        
        public int GetId(){return _id;}
    }
}
