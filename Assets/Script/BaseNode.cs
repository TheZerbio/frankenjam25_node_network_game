using System;
using UnityEngine;

namespace Script
{
    public class BaseNode : Node
    {
        const int CAPACITY = 0; 
        const float WORK_RADIUS = 120;
        const float VISION_RADIUS = 150;
        const float CONNECTION_RADIUS = 50f;

        public const int worker_cost = Int32.MaxValue;


        public BaseNode()
            : base(CAPACITY, WORK_RADIUS, VISION_RADIUS, CONNECTION_RADIUS)
        {
            
        }

        void Update()
        {
            var sprite = GetComponent<SpriteRenderer>();
            sprite.color = !isSelected ? DefaultColor : HighlightColor;
        }
        
        public override void Start()
        {
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
