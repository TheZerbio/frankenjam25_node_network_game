using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Script
{
    public class AntagonistNode : MonoBehaviour
    {
        public SubordinateState State;
        private ISelectable _element;
        private Node _elementAsNode;
        private Worker _elementAsWorker;
        public bool isBaseNote = false;

        private AntagonistController _controller;
        public List<SubordinateAction> actions = new List<SubordinateAction>();
        
        
        public Vector2 GetPos() => _element.getGameObject().transform.position;

        public ClickableType getType() => ClickableType.Node;

        public Node GetNode() => _elementAsNode ?? _element.getGameObject().GetComponent<Node>();
        public Worker GetWorker() => _elementAsWorker ?? _element.getGameObject().GetComponent<Worker>();
        
        private void Start()
        {
            _element = gameObject.GetComponent<ISelectable>();
            isBaseNote = _element.GetElementType() == ClickableType.Node && GetNode().lemmingCapacity == 0;
            
            var fraction = _element.GetFraction();
            
            _controller = AntagonistReferer.GetControllerByID(fraction);
            _controller.RegisterNode(this);
            
        }

        private void FixedUpdate()
        {
            if (isBaseNote) return;
            _controller ??= AntagonistReferer.GetControllerByID(_element.GetFraction());
            if (actions.Count == 0) _controller.RequestAction(this);
            if (actions.Count == 0) return;
            
            var action = actions[0];
            bool completed = false;
            
            State = action.State;
            
            switch (action.State)
            {
                case SubordinateState.ActionTo:
                    
                    action.Target = GetElementAt(action.GetPosition());
                    if (action.Target != null) 
                        goto case SubordinateState.ConnectWith;
                    
                    if (_element.OnActionToVoid(action.GetPosition()))
                        completed = true;
                    break;
                case SubordinateState.ConnectWith:
                    // todo check if target is alive
                    if (_element.OnActionToElement(action.Target))
                        completed = true;
                    break;
                case SubordinateState.Produce:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (completed) actions.RemoveAt(0);
        }

        private ISelectable GetElementAt(Vector2 pos)
        {
            var probe = Physics2D.Raycast(pos, Vector2.zero);
            return probe.collider != null ? probe.collider.GetComponent<ISelectable>() : null;
        }
    }
}