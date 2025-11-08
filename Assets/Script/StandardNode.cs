using UnityEngine;

namespace Script
{
    public class StandardNode : Node
    {
        const int CAPACITY = 20; 
        const float WORK_RADIUS = 20f;
        const float VISION_RADIUS = 30f;
        const float CONNECTION_RADIUS = 10f;

        public const int worker_cost = 10;


        public StandardNode(Vector2 position)
            : base(position, CAPACITY, WORK_RADIUS, VISION_RADIUS, CONNECTION_RADIUS)
        {
            
        }
        
        
        
    }
}