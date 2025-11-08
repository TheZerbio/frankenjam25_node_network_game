using Unity.VisualScripting;
using UnityEngine;

public interface IClickable
{
    public void OnSelect();
    public void OnClickToVoid(Vector2 position);
}
