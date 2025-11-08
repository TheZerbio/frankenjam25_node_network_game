using System;
using System.Net;
using Script;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Node: MonoBehaviour, ISelectable
{
    public int id { get; private set; }
    [SerializeField] public int fractionID = -1;
    public int lemmingCapacity { get; set; } = 0;
    public int lemmingCount { get; }
    
    // radia
    public float workRadius { get; set; } = 30;
    public float visionRadius { get; set; } = 50;
    public float connectionRadius { get; set; } = 25;

    private Rigidbody2D _rigidbody;
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
    
    public ClickableType GetElementType()
    {
        return ClickableType.Node;
    }
    
    public virtual void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
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
        GameManger.GetInstance().AddNodeToPlayerGraph(this,fractionID);
        
    }
    
    public void OnSelect()
    {
        isSelected = true;
        SetMDCIfPresent(true);
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
            GameManger manager = GameManger.GetInstance();
            manager.BuildNodeFromNode(this,position);
        }
    }

    public void OnActionToElement(ISelectable element) { }

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
            MDC.circles[0].radius = connectionRadius;
            MDC.circles[1].radius = workRadius;
            MDC.circles[2].radius = visionRadius;
            return true;
        }
        return false;
    }
    
}
