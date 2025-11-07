using UnityEngine;

public abstract class Node
{
    public long id { get; }
    public Vector2 position { get; private set; }
    public int lemmingCapacity { get; }
    public int lemmingCount { get; }
    
    public Rigidbody2D rigidbody { get; }
    
    // radia
    public float workRadius { get; }
    public float visionRadius { get; }
    public float connectionRadius { get; }
    
    

    public void pull(int numberOfPullers, Vector2 directionTowards)
    {
        rigidbody.AddForce(numberOfPullers * directionTowards.normalized);
    }
    
    
    


}
