using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;

public class Player : MonoBehaviour
{

    private Rigidbody2D _rigidbody2D;

    private Animator anim;

    [SerializeField] private bool isRunning_Anim_Bool;

    [Header("Move Info")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private bool Player_Unlock = false;

    [Header("Collision Info")]

    [SerializeField] private float groundCheck; 
    private bool isGrounded;
    [SerializeField] private LayerMask whatIsGround;


    // Start is called before the first frame update
    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Animator_Controller();

        if (Player_Unlock)
        {
            _rigidbody2D.velocity = new Vector2(moveSpeed, _rigidbody2D.velocity.y);
        }

        CheckCollision();

        CheckInput();
    }

    private void Animator_Controller()
    {
        isRunning_Anim_Bool = _rigidbody2D.velocity.x != 0;
        
        anim.SetBool("isRunning", isRunning_Anim_Bool);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("Y_Velocity", _rigidbody2D.velocity.y);
        anim.SetFloat("X_Velocity", _rigidbody2D.velocity.x);
    }

    private void CheckCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheck, whatIsGround);
    }

    private void CheckInput()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            Player_Unlock = true;
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