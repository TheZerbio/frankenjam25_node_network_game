using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ClickDetection : MonoBehaviour
{
    private List<ISelectable> _selected = new List<ISelectable>();
    private static ClickDetection _instance;

    public Camera mainCamera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple instances of ClickDetection. Only use one you Doofus");
            Destroy(gameObject);
        }
    }

    public static ClickDetection GetInstance()
    {
        return _instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(clickPosition, Vector2.zero);

            if (hit.collider != null )
            {
                ISelectable selectable = hit.collider.GetComponent<ISelectable>();
                if (selectable == null) { 
                    onVoidLeftClick();
                    return;
                }
                
                // if an clickable element is Left clicked
                onElemnentLeftClick(selectable);
            }
            else
            {
                onVoidLeftClick();
            }
            
        }
        
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(clickPosition, Vector2.zero);

            if (hit.collider != null )
            {
                ISelectable selectable = hit.collider.GetComponent<ISelectable>();
                if (selectable == null) { 
                    onVoidRightClick(clickPosition);
                    return;
                }
                
                // if an clickable element is Left clicked
                onElemnentRightClick(selectable);
            }
            else
            {
                onVoidRightClick(clickPosition);
            }
            
        }
    }

    private void onElemnentLeftClick(ISelectable element)
    {
        bool wasSelected = UnSelect(element);
        
        if (!ShiftPressed())
            UnSelectAll();

        if (!wasSelected)
          AddSelection(element);
    }

    private void onElemnentRightClick(ISelectable element)
    {
        foreach (var selectedElement in _selected)
        {
            selectedElement.OnActionToElement(element);
        }
    }
    
    private void onVoidLeftClick()
    {
        UnSelectAll();
    }

    private void onVoidRightClick(Vector2 position)
    {
        foreach (var selectedElement in _selected)
            selectedElement.OnActionToVoid(position);
    }

    private bool ShiftPressed()
    {
        return InputSystem.actions["Shift"].IsPressed();
    }

    private void AddSelection(ISelectable element)
    {
        _selected.Add(element);
        element.OnSelect();
    }

    private bool UnSelect(ISelectable element)
    {
        bool wasSelected = _selected.Remove(element);
        if (wasSelected) element.OnDeselect();

        return wasSelected;
    }

    private void UnSelectAll()
    {
        lock (_selected)
        {
            List<ISelectable> safetyCopy = new List<ISelectable>(_selected);
            foreach (var element in safetyCopy)
            {
                UnSelect(element);
            }
        }
    }

    public void changeSelected(List<ISelectable> unselect, List<ISelectable> select)
    {
        foreach (var removal in unselect)
            UnSelect(removal);
        foreach (var addition in select) 
            AddSelection(addition);
        
    }

}
