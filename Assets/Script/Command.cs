

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

        public Vector2 GetTargetPosition()
        {
            return targetPosition ?? targetObject.getGameObject().transform.position;
        }
        
        public bool IsObject() => targetObject != null;
        public bool IsWorker() => IsObject() && targetObject.GetElementType() == ClickableType.Lemming;
        public Worker getWorker()
        {
            return IsWorker()? targetObject as Worker : null;
        }
        public GameObject GetGameObject() => IsObject()?  targetObject.getGameObject() : null;
        
        
    }
}