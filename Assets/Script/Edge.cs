using UnityEngine;

public abstract class Edge
{
    public final int id;
    private Node  _node1;
    private Node  _node2;

    public bool Equals(Edge obj)
    {
        return id == obj.id;
    }
}
