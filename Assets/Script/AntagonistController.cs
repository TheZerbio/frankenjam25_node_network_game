using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using Unity.VisualScripting;
using UnityEngine;
using Math = Unity.Mathematics.Geometry.Math;

namespace Script
{
    public class AntagonistReferer
    {
        private static Dictionary<int, AntagonistController> _antagonistControllers = new Dictionary<int, AntagonistController>();
        private static Dictionary<Vector2, int> _nodeCreationMap = new Dictionary<Vector2, int>();

        private AntagonistReferer()
        { }

        private static AntagonistReferer singleton;

        public static AntagonistReferer getInstance()
        {
            singleton ??= new AntagonistReferer();
            return singleton;
        }

        public static void CreateNodeInNetwork(Node creator, Vector2 position)
        {
            if (creator.fractionID == GameManger.GetInstance().CurrentPlayer) return;
            if (_nodeCreationMap.Keys.Contains(position)) return;
            
            _nodeCreationMap.Add(position, creator.fractionID);
            creator.OnActionToVoid(position);
        }
        
        public static int GetFractionID(Node node)
        {
            node.fractionID =  _nodeCreationMap[node.transform.position];
            return node.fractionID;
        }

        public AntagonistController GetControllerByID(int fractionID)
        {
            if (_antagonistControllers.TryGetValue(fractionID, out var controller))
                return controller;
            
            // the Player should't be controlled
            if (fractionID == GameManger.GetInstance().CurrentPlayer) return null; 
            
            // create new controller and Mapping
            var antagonistController = new AntagonistController();
            _antagonistControllers.Add(fractionID, antagonistController);
            
            return antagonistController;
        }
    }
    
    public enum AntagonistState{
        Fortify, 
        Explore,
        Defend,
        Attack
    }

    public enum SubordinateState
    {
        ActionTo, ConnectWith, Wait, Produce 
    }

    public class SubordinateAction
    {
        public SubordinateState State;
        private readonly Vector2? _position;
        public readonly ISelectable Target;

        public SubordinateAction(SubordinateState state, Vector2? position, ISelectable target)
        {
            _position = position;
            State = state;
            Target = target;
        }
        
        public Vector2 GetPosition() => _position?? Target.getGameObject().transform.position;
    }
    
    public class AntagonistController
    {

        private class AntagonistAction
        {
            public AntagonistState state;
            public Vector2 position;
            public float priority = 1;
            public AntagonistAction(AntagonistState state, Vector2 position)
            {
                this.state = state;
                this.position = position;
            }
        }


        private List<Node> _networkNodes =  new List<Node>();
        private List<AntagonistAction> Goals { get; set; }

        public void RegisterNode(Node node)
        {
            
        }

        private AntagonistAction GetClosestAction(Vector2 position, bool worker = false)
        {
            
            // find Action with nearest Distance
            AntagonistAction bestAction = null;
            var bestDistance = float.PositiveInfinity;
            
            foreach (var goal in Goals.Where(goal => !worker || 
                            CanBeUsedByWorker(goal.state)).Where(goal => bestAction == null ||
                                                                         bestDistance > Vector2.Distance(goal.position, position)))
            {
                bestAction = goal;
                bestDistance = Vector2.Distance(goal.position, position);
            }

            return bestAction;
        }

        private static bool CanBeUsedByWorker(AntagonistState state)
        {
            return state != AntagonistState.Fortify && state != AntagonistState.Explore;
        }

        private void PlanFortifyLayer(Node center, Vector3 firstNodeOnLayerPos)
        {
            var centerPos = center.transform.position;
            
            var radius = Vector2.Distance(firstNodeOnLayerPos, center.transform.position);
            var segmentLength = 0.8f * center.workRadius;

            // calculate the angle between nodes by calculating the number of vertices and dividing the circle into
            // equal parts (the smallest Polynomial has 3 Vertices) 
            var numberOfNodes = math.max(Mathf.RoundToInt(Mathf.PI / Mathf.Asin(segmentLength / 2 * radius)), 3);
            var angleStep = 360f / numberOfNodes;
            
            for (var i = 1; i < numberOfNodes ; i++)
            {
                var angle = angleStep * i;
                Vector2 rotated = centerPos + Quaternion.Euler(0, 0, angle) * (firstNodeOnLayerPos - centerPos);
                Goals.Add(new AntagonistAction(AntagonistState.Fortify, rotated));
            }
        }

        private void PlanFortification(Node center, int levels)
        {
            var lastLayerStart = center.transform.position;
            var expansionVector = (-center.transform.position).normalized * 0.8f * center.workRadius;
            for (var layer = 0; layer < levels; layer++)
            {
                // find/ create first node 
                Vector3? firstNodeOnLayerPos = null;
                foreach (var node in _networkNodes)
                {
                    var nodeCenterDistance = Vector2.Distance(node.transform.position, center.transform.position);
                    if (nodeCenterDistance <= (lastLayerStart + expansionVector).magnitude &&
                        nodeCenterDistance > Vector2.Distance(lastLayerStart, center.transform.position))
                    {
                        firstNodeOnLayerPos = node.transform.position;
                        break;
                    }
                }

                lastLayerStart =  firstNodeOnLayerPos?? (lastLayerStart + expansionVector);
                    
                if (firstNodeOnLayerPos == null) 
                    Goals.Add(new AntagonistAction(AntagonistState.Fortify, lastLayerStart));
                
                PlanFortifyLayer(center, lastLayerStart);
            }
        }

        private List<SubordinateAction> ToSubActions(AntagonistAction action)
        {
            
            List<SubordinateAction> tasks = new List<SubordinateAction>();
            switch (action.state)
            {
                case AntagonistState.Fortify:
                    tasks.Add(new SubordinateAction(SubordinateState.ActionTo, action.position, null));
                    break;
                case AntagonistState.Explore:
                    break;
                case AntagonistState.Defend:
                    break;
                case AntagonistState.Attack:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }
        
        
        
        
        
    }
}