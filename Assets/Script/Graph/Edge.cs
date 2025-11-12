using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Script.Graph
{
    public abstract class Edge : MonoBehaviour
    {
        public static double MaxStrength;
        private int _id { get; } = GameManger.GetNextEdgeId();
        public Node  _node1;
        public Node  _node2;

        public bool Equals(Edge obj)
        {
            return (_node1 == obj._node1 && _node2 == obj._node2) || (_node1 == obj._node2 && _node2 == obj._node1);
        }

        public Edge()
        {
            _id = GameManger.GetNextEdgeId();
            _node1 = null;
            _node2 = null;
        }

        public virtual void Start()
        {
            
        }

        public virtual void Update()
        {
            if (_node1 == null || _node2 == null)
            {
                Destroy(gameObject);
                return;
            }
            if(CheckDissconect())
                GameManger.GetInstance().DestroyEdge(this);
        }

        public bool CheckDissconect()
        {
            return Vector2.Distance(_node1.transform.position,_node2.transform.position) > GetDissconectDistance();
        }
        
        public int GetId(){return _id;}

        public Node GetOtherNode(Node self)
        {
            if (self == _node1) return _node2;
            if (self == _node2) return _node1;
            return null;
        }

        public float GetDissconectDistance()
        {
            return Math.Min(this._node1.workRadius, this._node2.workRadius);
        }
        
    }
}
