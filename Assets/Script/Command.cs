

using UnityEngine;

namespace Script
{
    public class Command
    {
        
        public Vector2? targetPosition { get; }
        public ISelectable targetObject { get; }
        
        
        public Command(Vector2? targetposition, ISelectable targetobject )
        {
            this.targetPosition = targetposition;
            this.targetObject = targetobject;
        }
    }
}