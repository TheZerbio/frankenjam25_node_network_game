using System;
using Unity.Mathematics;
using UnityEngine;

public class Worker : MonoBehaviour, IClickable
{
    public long id;
    Color _defaultColor = Color.darkOrange;
    Color _highlightColor = Color.darkCyan;
    private const float STANDARD_PULL_FORCE = 1f;
    private SpriteRenderer _renderer;

    private float _speed = 2f;

    private Vector2 _target;
    private Rigidbody2D _rigidbody;

    public static long selectedId = -1;
    public float pull_force = STANDARD_PULL_FORCE;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedId == id)
        {
            _renderer.color = _highlightColor;
        }
        else
        {
            _renderer.color = _defaultColor;
        }

        if (_target != Vector2.zero)
        {
            Vector2 tickMovement = Vector2.MoveTowards(_rigidbody.position,_target, _speed * Time.deltaTime);
            _rigidbody.MovePosition(tickMovement);
            if (Vector2.Distance(_target, _rigidbody.position) < 0.1f)
            {
                _target =Vector2.zero;
            }
        }
        
    }



    public void OnSelect()
    {
        if (selectedId != id) selectedId = id; else selectedId = -1;
    }

    public void OnClickToVoid(Vector2 position)
    {
        _target = position;
       
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
            Debug.Log("Collision Enter");
            FixedJoint2D joint = gameObject.AddComponent<FixedJoint2D>();
            joint.connectedBody = other.gameObject.GetComponent<Rigidbody2D>();
            joint.connectedAnchor = other.gameObject.transform.position;
            joint.breakForce = math.INFINITY;
            joint.breakTorque = math.INFINITY;
        
    }
}
