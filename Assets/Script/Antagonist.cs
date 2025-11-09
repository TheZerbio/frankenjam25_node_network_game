using System;
using System.Collections.Generic;
using Script.Graph;
using UnityEngine;

namespace Script
{

    enum State
    {
        Prepare, Attack, Fortify, Expand, Circle
    }
    public class Antagonist : MonoBehaviour
    {
        public int fractionID;
        
        public List<Edge> Edges = new List<Edge>();

        public Node currentNode;

        private static List<StandardNode> _nodes = new List<StandardNode>();
        public static List<Vector2> nextPositions = new List<Vector2>();
        
        State _state = State.Circle;

        private void Start()
        {
            
            nextPositions.Remove((Vector2)transform.position);
            
        }

        private void FixedUpdate()
        {
            switch (_state)
            {
                case State.Prepare:
                    break;
                case State.Attack:
                    break;
                case State.Fortify:
                    break;
                case State.Expand:
                    break;
                case State.Circle:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        
        private void Fortify()
        {
            if (Edges.Count >= 2)
            {
                // todo change state to expand
                return; 
            }

            foreach (var node in _nodes)
            {
                this.currentNode.OnActionToElement(node);
            }

            foreach (var pos in nextPositions)
            {
                currentNode.OnActionToVoid(pos);
            }
            
        }

        private void Expand()
        {
            
        }

        private List<Vector2> GetCirclePoints(Node centerNode, Vector2 startPosition)
        {
            
            Vector3 center = centerNode.transform.position;
            List<Vector2> result = new List<Vector2>();
            float angleStep = 360f / 8;

            for (int i = 1; i < 8; i++)
            {
                float angle = angleStep * i;
                Vector2 rotated = center + Quaternion.Euler(0, 0, angle) * ((Vector3)startPosition- center);
                result.Add(rotated);
            }

            return result;
            
        }
    }