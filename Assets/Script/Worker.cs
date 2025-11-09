using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Script;
using Script.Graph;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Worker : MonoBehaviour, ISelectable
{ 
    
    public long id;
    public ClickableType elementType { get; private set; }
    public int fractionID = 0;
    private float _force = 0.2f;
    
    // color change when selected 
    Color _defaultColor = Color.darkOrange;
    Color _highlightColor = Color.darkCyan;
    
    // selection variables 
    public bool isSelected = false;
    
    // movement 
    public float speed = 16f;
    public List<Command> NextCommands = new List<Command>();
    

    private bool _hasAI = true;
    
    
    private SpriteRenderer _renderer;
    private Rigidbody2D _rigidbody;

    public List<Worker> attachedWorkers { get; set; } = new List<Worker>();
    private float _stoppingDistance = 0.1f;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        elementType = ClickableType.Lemming;
        _renderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
        DisplayPathIfPossible();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // indicate Selection status by color
        _renderer.color = isSelected? _highlightColor : _defaultColor;

        Vector2? targetPosition = GetCurrentCommand().GetTargetPosition();
        // stop the motion when the target is reached
        if (Vector2.Distance((Vector2)targetPosition, _rigidbody.position) <= _stoppingDistance)
        {
            targetPosition = null;
        }
        
        if (targetPosition != null && _hasAI)
        {
            _stoppingDistance = 0.5f *  gameObject.GetComponent<Collider2D>().bounds.size.y;
            Vector2 applyForce = GetCurrentCommand().GetTargetPosition() - _rigidbody.position;
            _rigidbody.AddForce(_force * applyForce);
            float angle = Mathf.Atan2(applyForce.y, applyForce.x) * Mathf.Rad2Deg;
            _rigidbody.SetRotation(Mathf.MoveTowardsAngle(_rigidbody.rotation, angle, 180* Time.deltaTime));
            _rigidbody.linearVelocity = Vector2.ClampMagnitude(_rigidbody.linearVelocity, speed);
            
        }
        else
        {
            
            if (_hasAI)
            {
                if (targetPosition == null) applyNextCommand();
                _rigidbody.linearVelocity = Vector2.zero;
                _rigidbody.angularVelocity = 0f;
            }
        }

        if (isSelected && InputSystem.actions["Disconnect"].IsPressed())
        {
            DisAttach();
        }
        
    }


    public ClickableType GetElementType()
    {
        return ClickableType.Lemming;
    }

    /// ISelectable Implementation
    public void OnSelect() => isSelected =  true;
    
    public void OnDeselect() => isSelected = false;

    public void OnActionToVoid(Vector2 position) => registerNewCommand(position, null);
    

    public GameObject getGameObject() => gameObject;

    public void OnActionToElement(ISelectable element) 
    {
        registerNewCommand(null, element);
        if (element.GetElementType() == ClickableType.Lemming)
            ClickDetection.GetInstance().changeSelected(
                new List<ISelectable>(){this}, new List<ISelectable>(){element});
    }
   

    private void OnCollisionEnter2D(Collision2D other)
    {
        Command currentCommand = GetCurrentCommand();
        if ( other.gameObject == currentCommand.GetGameObject())
            _stoppingDistance = 1.5f * Vector2.Distance(transform.position, other.transform.position);
        
        var otherSelectable = other.gameObject.GetComponent<ISelectable>();
        if (otherSelectable == null) return;

        if (otherSelectable.GetElementType() == ClickableType.Lemming)
        {
            Worker coWorker = (Worker)otherSelectable;
            if (coWorker.fractionID != fractionID)
            {
                Worker.Destroy(this.gameObject);
                return;
            }

            if (currentCommand.IsObject() && other.gameObject == currentCommand.targetObject.getGameObject())
            {
                coWorker.attachedWorkers.Add(this);
                if (isSelected)
                    ClickDetection.GetInstance().changeSelected(
                        new List<ISelectable>(){this}, new List<ISelectable>(){coWorker});
            }
            
            
        } else if (otherSelectable.GetElementType() == ClickableType.Node)
        {
            Node otherNode = otherSelectable as Node;
            PullNode(otherNode);
        }
            
        
    }

    public float provideHelp(Worker applyer)
    {
        return applyer.getGameObject() == GetCurrentCommand().GetGameObject()? _force + AskForHelp() : 0;
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

    public bool getIsAttatchedToNode(Node node)
    {
        foreach (var joint in gameObject.GetComponents<FixedJoint2D>().ToList())
        {
            if (joint.connectedBody.gameObject == node.gameObject)
                return true;
        }
        return false;
    }

    private void PullNode(Node node)
    {
        Command currentCommand = GetCurrentCommand();
        if (!currentCommand.IsObject()) return;
        if ( currentCommand.GetGameObject() != node.getGameObject())
            if (currentCommand.IsWorker())
            {
                Worker coWorker = currentCommand.getWorker();

                if (coWorker.fractionID != fractionID  || !coWorker.getIsAttatchedToNode(node)) return;
                
                _hasAI = false;
                coWorker.attachedWorkers.Add(this);
                ClickDetection.GetInstance().changeSelected(
                    new List<ISelectable>(){this}, new List<ISelectable>(){coWorker});
            }
        
        
        FixedJoint2D joint = gameObject.AddComponent<FixedJoint2D>();
        joint.connectedBody = node.gameObject.GetComponent<Rigidbody2D>();
        joint.breakForce = math.INFINITY;
        joint.breakTorque = math.INFINITY;
        if (currentCommand.GetGameObject() != node.getGameObject()) _hasAI = false;
        else applyNextCommand();
    }

    private void DisconnectFromCommander()
    {
        if (GetCurrentCommand().IsObject() && GetCurrentCommand().targetObject.GetElementType() == ClickableType.Lemming)
        {
            GetCurrentCommand().getWorker().attachedWorkers.Remove(this);
        }
    }

    public void DisAttach()
    {
        List<FixedJoint2D> joints = new List<FixedJoint2D>(gameObject.GetComponents<FixedJoint2D>().ToList());
        bool hadJoints = joints.Count > 0;
        foreach (var joint in joints)
            Destroy(joint);
            
        if (!hadJoints) DisconnectFromCommander();
        else foreach ( var attachedWorker in attachedWorkers){ attachedWorker.DisAttach();}

        _hasAI = true;
    }

    private void applyNextCommand()
    {
        if (NextCommands.Count == 0)
        {
            return;
        }
        
        DisconnectFromCommander();
        NextCommands.RemoveAt(NextCommands.Count - 1);
    }

    private void registerNewCommand(Vector2? position, ISelectable element)
    {

        bool concat = InputSystem.actions["Concat"].IsPressed(); 
        bool concatTop = InputSystem.actions["ConcatTop"].IsPressed();
        
        if ( concat|| concatTop)
            NextCommands.Insert(concatTop ? NextCommands.Count: 0, new Command(position, element));
        else
        {
            NextCommands = new List<Command>(){new Command(position, element)};
        }
    }

    private void DisplayPathIfPossible()
    {
        var pathVisualizer = GetComponent<DrawDashedLineClass>();
        if (pathVisualizer)
        {
            pathVisualizer.worker = this;
        }
    }

    private Command GetCurrentCommand() => NextCommands.Count > 0? NextCommands[^1] :  new Command(transform.position, null);

}
