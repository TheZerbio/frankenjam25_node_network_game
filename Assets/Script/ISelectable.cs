using Unity.VisualScripting;
using UnityEngine;

public enum ClickableType
{
    Lemming,
    Node
}

public interface ISelectable
{
    public ClickableType GetElementType();
    public void OnSelect();
    public void OnDeselect();
    public void OnActionToVoid(Vector2 position);
    
    public void OnActionToElement(ISelectable element);
    
    public GameObject getGameObject();
    
    
}
