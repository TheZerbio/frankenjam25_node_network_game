using UnityEngine;
using UnityEngine.InputSystem;

public class Test : MonoBehaviour
{
    public int x = 5;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Hallo from Start");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Hallo from Update"+x);
        x++;
        var t = Time.deltaTime;
    }

    void FixedUpdate()
    {
        Debug.Log("Hallo from FixedUpdate");
        var rb = gameObject.GetComponent<Rigidbody2D>();
        if (InputSystem.actions["Jump"].IsPressed())
        {
            rb.AddForce(new Vector2(0, 10));
        }
        
    }
}
