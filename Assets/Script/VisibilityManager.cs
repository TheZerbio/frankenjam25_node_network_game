using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Script
{
    public class VisibilityManager : MonoBehaviour
    {
        private List<ISelectable> _watchList = new List<ISelectable>();
        public List<ISelectable> _visible = new List<ISelectable>();
        private ISelectable _thisElement;
        private AntagonistController _controller;

        private float _visionRange;

        private void Start()
        {
            _thisElement = GetComponent<ISelectable>();
            _controller = AntagonistReferer.GetControllerByID(_thisElement.GetFraction());
            _visionRange = _thisElement.GetElementType() == ClickableType.Lemming
                ? GetComponent<Worker>().visionRadius
                : GetComponent<Node>().visionRadius;
        }

        private void FixedUpdate()
        {
            foreach (var element in _watchList
                         .Where(element 
                             => Vector2.Distance(GetElementPosition(element), GetElementPosition(_thisElement))
                                < _visionRange))
                OnVisibilityEnter(element);
            foreach (var element in _visible
                         .Where(element
                             => element != null 
                                && Vector2.Distance(GetElementPosition(element), GetElementPosition(_thisElement)) 
                                >_visionRange))
                OnVisibilityExit(element);
            
        }

        private void OnTriggerEnter2D(Collider2D collisionObject)
        {
            var other = collisionObject.GetComponent<ISelectable>();
            if (other == null) return;
            
            if (other.GetFraction() == _thisElement.GetFraction()) return;
            
            if (_watchList.Contains(other)) return;
            _watchList.Add(other);
        }

        private void OnTriggerExit(Collider leavingObject)
        {
            var other = leavingObject.GetComponent<ISelectable>();
            if (other == null) return;
            
            _watchList.Remove(other);
            if (_visible.Remove(other)) OnVisibilityExit(other);
        }
        
        private void OnVisibilityEnter(ISelectable element)
        {
            _watchList.Remove(element);
            _visible.Add(element);
            _controller.onVisionEnter(element);
        }
        private void OnVisibilityExit(ISelectable element)
        {
            _visible.Remove(element);
            _controller.onVisionExit(element);
        }

        private Vector2 GetElementPosition(ISelectable element) => element.getGameObject().transform.position;
        
    }
    
    
    
    
    
}