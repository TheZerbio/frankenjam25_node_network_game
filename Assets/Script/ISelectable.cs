using System.Collections.Generic;
using Script;
using Unity.VisualScripting;
using UnityEngine;

public enum ClickableType
{
    Lemming,
    Node
}

public interface ISelectable
{
    public int GetFraction();
    public ClickableType GetElementType();
    public void OnSelect();
    public void OnDeselect();
    public bool OnActionToVoid(Vector2 position);
    
    public bool OnActionToElement(ISelectable element);
    
    public GameObject getGameObject();
    
    
}
