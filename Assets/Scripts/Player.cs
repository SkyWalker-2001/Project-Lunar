using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class Player : MonoBehaviour
{


    public float moveSpeed;
    public float jumpForce;

    public Rigidbody2D _rigidbody2D;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _rigidbody2D.velocity = new Vector2(moveSpeed, _rigidbody2D.velocity.y);

        if(Input.GetKeyDown(KeyCode.Space)){
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, jumpForce);
        }
    }
}
