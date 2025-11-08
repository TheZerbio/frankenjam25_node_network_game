using System;
using System.Collections.Generic;
using System.Linq;
using Script;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class Worker : MonoBehaviour, ISelectable
{ 
    
    public long id;
    public ClickableType elementType { get; private set; }
    public int fractionID = 0;
    private float _force = 1f;
    
    // color change when selected 
    Color _defaultColor = Color.darkOrange;
    Color _highlightColor = Color.darkCyan;
    
    // selection variables 
    public bool isSelected = false;
    
    // movement 
    public float speed = 2f;
    private Vector2? _target = null;
    
    
    private SpriteRenderer _renderer;
    private Rigidbody2D _rigidbody;

    private ISelectable _targetObject;
    public List<Worker> attachedWorkers { get; set; } = new List<Worker>();
    private float _stoppingDistance = 0.1f;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        elementType = ClickableType.Lemming;
        _renderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
        attachedWorkers.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        // indicate Selection status by color
        _renderer.color = isSelected? _highlightColor : _defaultColor;
        

        if (_targetObject != null)  _target = _targetObject.getGameObject().transform.position;
        
        // stop the motion when the target is reached
        if (_target != null && Vector2.Distance((Vector2)_target, _rigidbody.position) <= _stoppingDistance)
            _target = null;
        
        if (_target != null)
        {
            _stoppingDistance = 0.1f;
            Vector2 applyForce = (_target?? Vector2.negativeInfinity) - _rigidbody.position;
            _rigidbody.AddForce(applyForce);
            _rigidbody.linearVelocity = Vector2.ClampMagnitude(_rigidbody.linearVelocity, speed);
            
        }
        else
        {
            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.angularVelocity = 0f;
        }
        
    }


    public ClickableType GetElementType()
    {
        return ClickableType.Lemming;
    }

    /// ISelectable Implementation
    public void OnSelect() => isSelected =  true;
    
    public void OnDeselect() => isSelected = false;

    public void OnActionToVoid(Vector2 position)
    {
        _target = position;
        _targetObject =  null;
        _stoppingDistance = 0.1f;
    }

    public GameObject getGameObject() => gameObject;

    public void OnActionToElement(ISelectable element) => _targetObject = element;

    public void OnClickToVoid(Vector2 position) => _target = position;
   

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (_targetObject != null && other.gameObject == _targetObject.getGameObject())
            _stoppingDistance = 2 * Vector2.Distance(transform.position, other.transform.position);
        
        
        var otherSelectable = other.gameObject.GetComponent<ISelectable>();
        if (otherSelectable == null) return;

        if (otherSelectable.GetElementType() == ClickableType.Lemming)
        {
            Worker coWorker = (Worker)otherSelectable;
            if (coWorker.fractionID != fractionID)
            {
                Worker.Destroy(this, 500);
                return;
            }

            if (_targetObject != null && other.gameObject == _targetObject.getGameObject())
            {
                coWorker.attachedWorkers.Add(this);
                ClickDetection.GetInstance().changeSelected(new List<ISelectable>(){this}, new List<ISelectable>(){coWorker});
            }
            
            
        } else if (otherSelectable.GetElementType() == ClickableType.Node)
        {
            Node otherNode = otherSelectable as Node;
            PullNode(otherNode);
        }
            
        
    }

    public float provideHelp(Worker applyer)
    {
        return applyer.getGameObject() == _targetObject.getGameObject() ? _force + AskForHelp() : 0;
    }

    private float AskForHelp()
    {
        float attachedForce = 0;
        foreach(var helpers in attachedWorkers)
        {
            attachedForce += provideHelp(this);
        }

        return attachedForce;
    }

    private void PullNode(Node node)
    {
        FixedJoint2D joint = gameObject.AddComponent<FixedJoint2D>();
        joint.connectedBody = node.gameObject.GetComponent<Rigidbody2D>();
        joint.breakForce = math.INFINITY;
        joint.breakTorque = math.INFINITY;
    }
}
