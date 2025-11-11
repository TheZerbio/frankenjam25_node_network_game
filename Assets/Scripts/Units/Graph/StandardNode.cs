using System;
using System.Xml.Schema;
using Script.Units;
using TMPro;
using Units.Graph;
using Unity.VisualScripting;
using UnityEngine;

namespace Script
{
    public class StandardNode : Node
    {
        const int CAPACITY = 40; 
        const float WORK_RADIUS = 80f;
        const float VISION_RADIUS = 100f;

        public const int worker_cost = UnitConfig.WORKER_COST;


        public StandardNode()
            : base(CAPACITY, WORK_RADIUS, VISION_RADIUS)
        {
        }

        public override void Update()
        {
            base.Update();
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
            if (LastCount != edges.Count || edges.Count == 0)
            {
                LastCount = edges.Count;
                if (!IsConnectedToBaseNode())
                {
                    fractionID = -1;
                    PropagateFractioID();
                }
            }
            counter.text =
                $@"Nauts: {lemmingCount}/{lemmingCapacity}
                Atttacker Cost: {_workerCost}
                Edge Cost: {_edgeCost}
                Node Cost: {_NodeDuplicationCost}";
        }

        public override void OnCollisionEnter2D(Collision2D other)
        {
            base.OnCollisionEnter2D(other);
        }


        public void Configure()
        {
            
        }
        
    }
}