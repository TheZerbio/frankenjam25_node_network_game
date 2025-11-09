using System;
using System.Collections.Generic;
using Script;
using Script.Graph;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public abstract class Node: MonoBehaviour, ISelectable
{
    public int id { get; private set; }
    [SerializeField] public int fractionID = -1;
    public int lemmingCapacity { get; set; } = 40;
    public int lemmingCount = 30;
    
    public const float LEMMING_FORCE = 0.2f;
    public const float LEMMING_SPEED = 2.75f;
    public float regenerationRate = 0.02f;
    
    public List<Edge> edges { get; private set; } = new List<Edge>();
    protected int lastCount = 0;
    
    // radia
    public float workRadius { get; set; } = 30;
    public float visionRadius { get; set; } = 50;
    public float connectionRadius { get; set; } = 25;

    private int _NodeDuplicationCost = 15;
    private int _workerCost = 5;
    private int _edgeCost = 10;
    
    public GameObject workerPrefab;
    private bool _workerSpawned = false;

    private Rigidbody2D _rigidbody;
    private Vector2 _restPosition;
    protected bool isSelected;
    protected Color DefaultColor;
    protected Color HighlightColor;

    protected Node(
        int lemmingCapacity,
        float workRadius, 
        float visionRadius,
        float connectionRadius) 
    {
        id = GameManger.GetNextNodeId();
        this.lemmingCapacity = lemmingCapacity;
        this.visionRadius = visionRadius;   
        this.connectionRadius = connectionRadius;
        this.workRadius = workRadius;
    }

    public int GetFraction() => fractionID;


    public ClickableType GetElementType()
    {
        return ClickableType.Node;
    }
    
    public virtual void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        if (fractionID != -1) _restPosition = transform.position; // todo call whenever the fraction changes
        
        GameManger.GetInstance().AddNodeToPlayerGraph(this,fractionID);
        
    }
    
    public void OnSelect()
    {
        isSelected = true;
        SetMDCIfPresent(true);
    }

    public virtual void Update()
    {
        RefreshColours();
    }

    public virtual void FixedUpdate()
    {
        RegeneratePopulation();
        if (  fractionID != -1 && Vector2.Distance(transform.position, _restPosition) > 0.5 * connectionRadius)
        {
            float managerBonus = isSelected ? 1.1f : 0.8f;
            Vector2 delta = _restPosition - (Vector2)transform.position;
            _rigidbody.AddForce(lemmingCount * LEMMING_FORCE * managerBonus * delta.normalized);
        }

        if (isSelected && InputSystem.actions["Spawn"].IsPressed() )
        {
            if (!_workerSpawned) SpawnWorker();
            _workerSpawned = true;
        }
        else _workerSpawned = false;


    }

    public void OnDeselect()
    {
        isSelected = false;
        SetMDCIfPresent(false);
    }

    public void OnActionToVoid(Vector2 position)
    {
        if (Vector2.Distance(position, gameObject.transform.position) < workRadius)
        {
            if (lemmingCount < (_NodeDuplicationCost+_edgeCost))
            {
                Debug.Log("Can't Afford that new node there, Ey?");
                /// todo send user a message that he hasn't enough Lemmings
                return;
            }

            Debug.Log("Trying to build Node from Node");
            lemmingCount -= _NodeDuplicationCost+_edgeCost;
            
            GameManger manager = GameManger.GetInstance();
            manager.BuildNodeFromNode(this,position);
        }
    }

    public void OnActionToElement(ISelectable element)
    {
        if (element is Node)
        {
            if (lemmingCount < _edgeCost)
            {
                Debug.Log("Imagine being too poor to build an edge... couldn't be me.");
                // todo Anti Edgy User message
                return;
            }

            lemmingCount -= _edgeCost;
            var other = (Node)element;
            if (Vector2.Distance(transform.position, other.transform.position) <= workRadius)
            {
                if (other.fractionID == -1)
                {
                    other.fractionID = fractionID;
                    other.PropagateFractioID();
                }
                if (!GameManger.GetInstance().CreateEdge(this, other, fractionID))
                    Debug.Log("Could create edge because of Faction issues");
                
            }
                
        }
    }

    public GameObject getGameObject() =>  gameObject;

    public void SetMDCIfPresent(bool newStatus)
    {
        var MDC = GetComponent<MultiDashedCircles>();
        if (!MDC)
        {
            Debug.Log("Node "+id+" doesn't have MDC component");
            return;
        }
        MDC.SetCirclesVisible(newStatus);
    }

    protected bool ConfigureMDCIfPresent()
    {
        var MDC = GetComponent<MultiDashedCircles>();
        if (MDC)
        {
            MDC.circles[0].radius = workRadius;
            MDC.circles[1].radius = visionRadius;
            if (this is BaseNode)
                MDC.circles[0].radius = 0;
            return true;
        }
        return false;
    }

    protected void RefreshColours()
    {
        try
        {
            if (fractionID == -1)
            {
                DefaultColor = Color.white;
                HighlightColor = Color.lightGray;
                return;
            }
            DefaultColor = GameManger.GetInstance().colors[fractionID];
            HighlightColor = GameManger.GetInstance().highlightedColors[fractionID];
        }
        catch (IndexOutOfRangeException e)  
        {
            
            Debug.LogError("Node.cs: No Color set for this factionID: "+e);
        }
    }


    public virtual void OnCollisionEnter2D(Collision2D other)
    {
        ISelectable otherSelectable = other.collider.GetComponentInParent<ISelectable>();
        if (otherSelectable == null) return;

        if (otherSelectable.GetElementType() == GetElementType())
        {
            KillAllEdges();
            Destroy(this.gameObject);
        }
    }

    private void RegeneratePopulation()
    {
        if (fractionID == -1) { return;}
        int lemmingIncrease = (Random.Range(0f, 1f) <= (lemmingCount* 0.7f) / (float)lemmingCapacity * regenerationRate 
            + 0.001 * regenerationRate)? 1 : 0;
        lemmingCount = Math.Min(lemmingCount + lemmingIncrease, lemmingCapacity);
    }


    private void SpawnWorker()
    {
        if (lemmingCount < _workerCost)
        {
            Debug.Log("No more money for Workers!");
            /// todo scold the user because hes poor
            return;
        }

        lemmingCount -= _workerCost;

        GameObject root = GameObject.Find("Workers");
        if (root == null)
        {
            root = new GameObject("Workers");
        }
        
        
        GameObject workerGO = Instantiate(workerPrefab, transform.position, Quaternion.identity);
        workerGO.transform.SetParent(root.transform, worldPositionStays: true);
        Worker worker = workerGO.GetComponent<Worker>();

        if (worker == null)
        {
            Debug.LogError("Node.cs: Incorrect prefab assigned to create a worker");
            return;
        }

        worker.fractionID = fractionID;
        worker.speed = LEMMING_SPEED * _workerCost;
        worker.force = LEMMING_FORCE * _workerCost;
        worker.DefaultColor = DefaultColor;
        worker.HighlightColor = HighlightColor;

    }
    
    public bool IsConnectedToBaseNode()
    {
        var visited = new HashSet<Node>();
        var queue = new Queue<Node>();
        queue.Enqueue(this);
        visited.Add(this);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            // ✅ Check if the node is a BaseNode
            if (current is BaseNode)
                return true;

            foreach (var edge in current.edges)
            {
                var neighbor = edge.GetOtherNode(current);
                if (neighbor != null && !visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        return false; // No BaseNode found in reachable nodes
    }

    public void PropagateFractioID()
    {
        var visited = new HashSet<Node>();
        var queue = new Queue<Node>();
        queue.Enqueue(this);
        visited.Add(this);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            // ✅ Check if the node is a BaseNode
            current.fractionID = fractionID;

            foreach (var edge in current.edges)
            {
                var neighbor = edge.GetOtherNode(current);
                if (neighbor != null && !visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }
    }
    
    void KillAllEdges()
    {
        var edgeArray = edges.ToArray();
        edges = new List<Edge>();
        for(int i = 0; i < edgeArray.Length; i++)
        {
            GameManger.GetInstance().DestroyEdge(edgeArray[i]);
        }
    }
    
}
