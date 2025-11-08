using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class Worker : MonoBehaviour, ISelectable
{ 
    
    public long id;
    public ClickableType elementType { get; private set; }
    public int fractionID = 0;
    public float force = 1f;
    
    // color change when selected 
    Color _defaultColor = Color.darkOrange;
    Color _highlightColor = Color.darkCyan;
    
    // selection variables 
    public bool isSelected = false;
    
    // movement 
    public float speed = 2f;
    private Vector2 _target;
    
    
    private SpriteRenderer _renderer;
    private Rigidbody2D _rigidbody;

    private GameObject _targetObject;
    private List<Worker> attatchedWorkers = new List<Worker>();

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        elementType = ClickableType.Lemming;
        _renderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
        attatchedWorkers.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        // indicate Selection status by color
        _renderer.color = isSelected? _highlightColor : _defaultColor;
        

        if (_targetObject) _target = _targetObject.transform.position;
        if (_target != Vector2.zero)
        {
            Vector2 applyForce = _target - _rigidbody.position;
            _rigidbody.AddForce(applyForce);
            _rigidbody.linearVelocity = Vector2.ClampMagnitude(_rigidbody.linearVelocity, speed);
            
            if (Vector2.Distance(_target, _rigidbody.position) < 0.1f)
                _target =Vector2.zero;
            
        } else _rigidbody.linearVelocity = Vector2.zero;
        
    }

    
    /// ISelectable Implementation
    public void OnSelect() => isSelected =  true;
    
    public void OnDeselect() => isSelected = false;

    public void OnActionToVoid(Vector2 position) => _target = position;

    public GameObject getGameObject() => gameObject;

    public void OnActionToElement(ISelectable element) => _targetObject = element.getGameObject();

    public void OnClickToVoid(Vector2 position) => _target = position;
   

    private void OnCollisionEnter2D(Collision2D other)
    {
        return;
            Debug.Log(other.gameObject.name);
            FixedJoint2D joint = gameObject.AddComponent<FixedJoint2D>();
            joint.connectedBody = other.gameObject.GetComponent<Rigidbody2D>();
            joint.breakForce = math.INFINITY;
            joint.breakTorque = math.INFINITY;
            
            
            // create/merge clusters 
            
        
    }
}
