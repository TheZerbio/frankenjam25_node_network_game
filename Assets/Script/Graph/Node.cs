using Script;
using UnityEngine;

public abstract class Node: MonoBehaviour, ISelectable
{
    public long id { get; }
    public int fractionID { get; set; }
    public Vector2 position { get; private set; }
    public int lemmingCapacity { get; } = 0;
    public int lemmingCount { get; }
    
    // radia
    public float workRadius { get; }
    public float visionRadius { get; }
    public float connectionRadius { get; }

    private Rigidbody2D _rigidbody;

    public Node(
        Vector2 position, 
        int lemmingCapacity,
        float workRadius, 
        float visionRadius,
        float connectionRadius)
    {
        id = GameManger.GetNextNodeId();
        _rigidbody = GetComponent<Rigidbody2D>();
        this.position = position;
        this.lemmingCapacity = lemmingCapacity;
        this.visionRadius = visionRadius;   
        this.connectionRadius = connectionRadius;
        this.workRadius = workRadius;
    }

    public ClickableType GetElementType()
    {
        return ClickableType.Node;
    }

    public void OnSelect(){}
  

    public void OnDeselect() { }

    public void OnActionToVoid(Vector2 position) { }

    public void OnActionToElement(ISelectable element) { }

    public GameObject getGameObject() =>  gameObject; 
    
}
