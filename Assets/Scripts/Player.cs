using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;

public class Player : MonoBehaviour
{

    public Rigidbody2D _rigidbody2D;

    [Header("Move Info")]
    public float moveSpeed;
    public float jumpForce;
    public bool runBegin = false;

    [Header("Collision Info")]

    public float groundCheck; 
    private bool isGrounded;
    public LayerMask whatIsGround;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (runBegin)
        {
            _rigidbody2D.velocity = new Vector2(moveSpeed, _rigidbody2D.velocity.y);
        }

        CheckCollision();

        CheckInput();
    }

    private void CheckCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheck, whatIsGround);
    }

    private void CheckInput()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            runBegin = true;
        }

        // if(Input.GetKeyDown(KeyCode.Space)){
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, jumpForce);
        }
    }

    public void OnDrawGizmos(){
        Gizmos.DrawLine(transform.position,new Vector2(transform.position.x, transform.position.y - groundCheck));
    }
}