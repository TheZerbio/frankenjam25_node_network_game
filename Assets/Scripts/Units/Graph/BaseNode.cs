using System;
using Units.Graph;
using UnityEngine;

namespace Script
{
    public class BaseNode : Node
    {
        const int CAPACITY = 0; 
        const float WORK_RADIUS = 100;
        const float VISION_RADIUS = 150;

        public const int worker_cost = Int32.MaxValue;
        public Node starterConnection;


        public BaseNode()
            : base(CAPACITY, WORK_RADIUS, VISION_RADIUS)
        {
            lemmingCapacity = 0;
        }

        public override void Update()
        {
            base.Update();
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

        public override void OnCollisionEnter2D(Collision2D collision)
        {
            base.OnCollisionEnter2D(collision);
        }

        
        public void Configure()
        {
            var _rigidbody = GetComponent<Rigidbody2D>();
            if (_rigidbody)
                _rigidbody.bodyType = RigidbodyType2D.Static;
        }
    }
}
