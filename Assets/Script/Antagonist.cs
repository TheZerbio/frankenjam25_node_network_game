using System;
using System.Collections.Generic;
using Script.Graph;
using UnityEngine;
using Random = System.Random;

namespace Script
{

    enum State
    {
        Prepare,
        Attack,
        Fortify,
        Connect,
        Expand,
        Circle,
        Sink, 
        
    }

    public class Antagonist : MonoBehaviour
    {
        public List<Edge> Edges = new List<Edge>();
        private int _nodeID = 0;

        public Node currentNode;

        private static List<Node> _nodes = new List<Node>();
        public static List<Vector2> nextPositions = new List<Vector2>();

        State _state = State.Circle;

        private void Start()
        {
            nextPositions.Remove((Vector2)transform.position);
            currentNode = gameObject.GetComponent<Node>();
            

            // if it is the base note
            if (currentNode.lemmingCapacity == 0)
            {
                _nodes.Insert(0, currentNode);
                _state = State.Sink;
            }
            else
            {
                _nodeID = _nodes.Count == 0? 1 : _nodes.Count;
                 _state = State.Circle;
                 _nodes.Add(currentNode);
            }
            // if it is the first operational node
            if (_nodes.Count == 2 && nextPositions.Count == 0)
            {
                nextPositions = GetCirclePoints(_nodes[0], _nodes[1].transform.position);
            }
            
           
        }

        private void FixedUpdate()
        {
            switch (_state)
            {
                case State.Sink: return;
                case State.Prepare:
                    break;
                case State.Attack:
                    break;
                case State.Fortify:
                    Fortify();
                    break;
                case State.Expand:
                    break;
                case State.Circle:
                    Circle();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Circle()
        {
            if (currentNode.lemmingCount > currentNode.NodeDuplicationCost)
            {
                if (nextPositions.Count > 0)
                    currentNode.OnActionToVoid(nextPositions[0]);
                _state = State.Fortify;
            }
        }


        private void Fortify()
        {
            if (currentNode.lemmingCount < currentNode._edgeCost) return;
                currentNode.OnActionToElement(_nodes[_nodeID-1]);

                if (currentNode.lemmingCount < currentNode._edgeCost) return;
            currentNode.OnActionToElement(_nodes[0]);
            if (_nodeID == 6)
            {
                if (currentNode.lemmingCount < currentNode._edgeCost) return;
                currentNode.OnActionToElement(_nodes[1]);
            }
            
            _state = State.Expand;
        }

        private void Expand()
        {

        }

        private List<Vector2> GetCirclePoints(Node centerNode, Vector2 startPosition)
        {

            Vector3 center = centerNode.transform.position;
            List<Vector2> result = new List<Vector2>();
            float angleStep = 360f / 6;

            for (int i = 1; i < 6; i++)
            {
                float angle = angleStep * i;
                Vector2 rotated = center + Quaternion.Euler(0, 0, angle) * ((Vector3)startPosition - center);
                result.Add(rotated);
            }

            return result;

        }
    }
}