using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ClickDetection : MonoBehaviour
{
    private IClickable _selected;

    public Camera mainCamera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(clickPosition, Vector2.zero);

            if (hit.collider != null)
            {
                IClickable clickable = hit.collider.GetComponent<IClickable>();
                if (_selected != clickable) _selected = clickable;
                else _selected = null;
                if (clickable != null) clickable.OnSelect();
            }
            else
            {
                if (_selected != null)
                    _selected.OnClickToVoid(clickPosition);
            }
            
        }
    }
    
}
