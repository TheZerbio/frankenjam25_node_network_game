using System;
using UnityEngine;

namespace Script
{
    public class BaseNode : Node
    {
        const int CAPACITY = 0; 
        const float WORK_RADIUS = 100;
        const float VISION_RADIUS = 150;
        const float CONNECTION_RADIUS = 80f; // Should be same as Standard Node Work radius

        public const int worker_cost = Int32.MaxValue;
        public Node starterConnection;


        public BaseNode()
            : base(CAPACITY, WORK_RADIUS, VISION_RADIUS, CONNECTION_RADIUS)
        {
            lemmingCapacity = 0;
        }

        public override void Update()
        {
            base.Update();
            var sprite = GetComponent<SpriteRenderer>();
            sprite.color = !isSelected ? DefaultColor : HighlightColor;
            if (edges.Count == 0)
                fractionID = -1;
        }
        
        public override void Start()
        {
            if(starterConnection)
                GameManger.GetInstance().CreateEdge(this, starterConnection,fractionID);
            Configure();
            base.Start();
            ConfigureMDCIfPresent();
        }

        
        public void Configure()
        {
            var _rigidbody = GetComponent<Rigidbody2D>();
            if (_rigidbody)
            {
                _rigidbody.bodyType = RigidbodyType2D.Static;
            }
        }
    }
}
