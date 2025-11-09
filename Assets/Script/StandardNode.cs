using System;
using System.Xml.Schema;
using Unity.VisualScripting;
using UnityEngine;

namespace Script
{
    public class StandardNode : Node
    {
        const int CAPACITY = 20; 
        const float WORK_RADIUS = 60f;
        const float VISION_RADIUS = 75f;
        const float CONNECTION_RADIUS = 40f;

        public const int worker_cost = 10;


        public StandardNode()
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

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        
        public void Configure()
        {
            //ConfigureBaseNode(lemmingCapacity, workRadius, visionRadius, connectionRadius);
        }
    }
}