using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using Unity.VisualScripting;
using UnityEngine;
using Math = Unity.Mathematics.Geometry.Math;
using Random = UnityEngine.Random;


namespace Script
{
    public static class AntagonistReferer
    {
        private static Dictionary<int, AntagonistController> _antagonistControllers = new Dictionary<int, AntagonistController>();
        private static Dictionary<Vector2, int> _nodeCreationMap = new Dictionary<Vector2, int>();
        
        public static void CreateNodeInNetwork(Node creator, Vector2 position)
        {
            if (creator.fractionID == GameManger.GetInstance().CurrentPlayer) return;
            if (_nodeCreationMap.Keys.Contains(position)) return;
            
            _nodeCreationMap.Add(position, creator.fractionID);
            creator.OnActionToVoid(position);
        }
        
        public static int GetFractionID(AntagonistNode node)
        {
            var fractionID =  _nodeCreationMap[node.GetPos()];
            return fractionID;
        }

        public static AntagonistController GetControllerByID(int fractionID)
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
        ActionTo, ConnectWith, Produce 
    }

    public class SubordinateAction
    {
        public SubordinateState State;
        private Vector2? _position;
        public ISelectable Target;

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
            public readonly AntagonistState State;
            private Vector2? _position;
            private ISelectable _target;
            public readonly float Priority = 1;
            public AntagonistAction(AntagonistState state, Vector2? position = null, ISelectable target = null, float priority = 1)
            {
                this.State = state;
                this._position = position;
                this.Priority = priority;
            }
            public Vector2 GetPos() => _position?? _target.getGameObject().transform.position;
        }


        private List<AntagonistNode> _networkNodes =  new List<AntagonistNode>();
        private List<AntagonistAction> Goals = new List<AntagonistAction>();

        public void RegisterNode(AntagonistNode actor)
        {
            if (actor.getType() == ClickableType.Node)
                if ((actor.isBaseNote && _networkNodes.Count > 0) || _networkNodes.Count == 1) 
                    PlanFortification(actor.GetNode(), true? 3 : Random.Range(1, 3));

            for (int goal_index = 0; goal_index < Goals.Count; goal_index++)
            {
                var goal = Goals[goal_index];
                if (goal.GetPos() != actor.GetPos()) continue;

                switch (goal.State)
                {
                    case AntagonistState.Fortify:
                        Goals.RemoveAt(goal_index--);
                        actor.actions.AddRange(_networkNodes
                            .Where(node => 
                                Vector2.Distance(node.GetPos(), actor.GetPos())
                                <= actor.GetNode().workRadius)
                            .Select(
                                node => new SubordinateAction(SubordinateState.ConnectWith, null, node.GetNode())));
                        break;
                    
                    default:
                        break;
                }
                
            }
            
            
            _networkNodes.Add(actor);
        }

        public void RequestAction(AntagonistNode actor)
        {
            var action = GetClosestAction(actor.GetPos(), actor.getType() == ClickableType.Lemming);
            actor.actions = ToSubActions(actor, action);
        }

        private AntagonistAction GetClosestAction(Vector2 position, bool worker = false)
        {
            
            // find Action with nearest Distance
            AntagonistAction bestAction = null;
            var bestDistance = float.PositiveInfinity;
            
            foreach (var goal in Goals.Where(goal => !worker || 
                            CanBeUsedByWorker(goal.State)).Where(goal => bestAction == null ||
                                                                         bestDistance > Vector2.Distance(goal.GetPos(), position) / goal.Priority))
            {
                bestAction = goal;
                bestDistance = Vector2.Distance(goal.GetPos(), position) / goal.Priority;
            }

            return bestAction;
        }

        private static bool CanBeUsedByWorker(AntagonistState state)
            => state != AntagonistState.Fortify && state != AntagonistState.Explore;

        private void PlanFortifyLayer(Node center, Vector3 firstNodeOnLayerPos, int layer = 1)
        {
            var centerPos = center.transform.position;
            
            var radius = Vector2.Distance(firstNodeOnLayerPos, center.transform.position);
            var segmentLength = 0.8f * center.workRadius;

            // calculate the angle between nodes by calculating the number of vertices and dividing the circle into
            // equal parts (the smallest Polynomial has 3 Vertices) 
            var numberOfNodes = math.max(Mathf.RoundToInt(Mathf.PI / Mathf.Asin(segmentLength / (2 * radius))), 3);
            var angleStep = 360f / numberOfNodes;
            
            for (var i = 1; i < numberOfNodes ; i++)
            {
                var angle = angleStep * i;
                Vector2 rotated = centerPos + Quaternion.Euler(0, 0, angle) * (firstNodeOnLayerPos - centerPos);
                Goals.Add(new AntagonistAction(AntagonistState.Fortify, rotated, priority:1f/ layer));
            }
        }

        private void PlanFortification(Node center, int levels)
        {
            var lastLayerStart = center.transform.position;
                        for (var layer = 0; layer < levels; layer++)
            {
                // find/ create first node 
                Vector3? firstNodeOnLayerPos = null;
                var expansionVector = (lastLayerStart-center.transform.position).normalized * 0.8f * center.workRadius;

                foreach (var node in _networkNodes)
                {
                    var nodeCenterDistance = Vector2.Distance(node.GetPos(), center.transform.position);
                    var lastCenterDistance = Vector2.Distance(lastLayerStart, center.transform.position);
                    if (nodeCenterDistance <= lastCenterDistance + center.workRadius &&
                        nodeCenterDistance > lastCenterDistance  + center.connectionRadius)
                    {
                        firstNodeOnLayerPos = node.GetPos();
                        break;
                    }
                }

                lastLayerStart =  firstNodeOnLayerPos?? (lastLayerStart + expansionVector);
                    
                if (firstNodeOnLayerPos == null) 
                    Goals.Add(new AntagonistAction(AntagonistState.Fortify, lastLayerStart,priority:1f/ layer));
                
                PlanFortifyLayer(center, lastLayerStart,  layer);
            }
        }

        private List<SubordinateAction> ToSubActions(AntagonistNode actor, AntagonistAction action)
        {
            
            var tasks = new List<SubordinateAction>();
            if (action == null) return tasks;
            
            switch (action.State)
            {
                case AntagonistState.Fortify:
                    tasks.Add(new SubordinateAction(SubordinateState.ActionTo, action.GetPos(), null));
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

            return tasks;
        }

        public void onVisionEnter(ISelectable detectedElement){}
        public void onVisionExit(ISelectable detectedElement){}




    }
}