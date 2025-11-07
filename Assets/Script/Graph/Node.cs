using UnityEngine;

public abstract class Node
{
    public abstract long id { get; }
    public Vector2 position { get; private set; }
    public int lemmingCapacity { get; }
    public int lemmingCount { get; }
    
    // radia
    public float workRadius { get; }
    public float visionRadius { get; }
    public float connectionRadius { get; }
    
    
    
    
}
