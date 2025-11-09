using System;
using System.Xml.Schema;
using Unity.VisualScripting;
using UnityEngine;

namespace Script
{
    public class StandardNode : Node
    {
        const int CAPACITY = 20; 
        const float WORK_RADIUS = 80f;
        const float VISION_RADIUS = 100f;
        const float CONNECTION_RADIUS = 40f;

        public const int worker_cost = 10;


        public StandardNode()
            : base(CAPACITY, WORK_RADIUS, VISION_RADIUS, CONNECTION_RADIUS)
        {
            
        }

        public override void Update()
        {
            base.Update();
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
            if (lastCount != edges.Count || edges.Count == 0)
            {
                lastCount = edges.Count;
                if (!IsConnectedToBaseNode())
                {
                    fractionID = -1;
                    PropagateFractioID();
                }
            }
        }

        
        public void Configure()
        {
            
        }
    }
}