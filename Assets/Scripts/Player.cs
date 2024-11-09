using System;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Components
    private Rigidbody2D _rigidbody2D;  // Reference to the Rigidbody2D component for physics-based movement
    private Animator anim;             // Reference to the Animator component for controlling animations

    // Animator parameters
    [SerializeField] private bool isRunning_Anim_Bool; // Animation boolean to trigger running animation


    [Header("Move Info")]
    [SerializeField] private float moveSpeed;          // Horizontal movement speed of the player
    [SerializeField] private float jumpForce;          // Force applied when the player jumps
    [SerializeField] private float doubleJumpForce;    // Force applied when performing a double jump

    private bool can_Double_Jump;      // Tracks if the player can perform a double jump

    [SerializeField] private bool Player_Unlock = false; // Unlocks player movement upon input

    [Header("Slide Info")]
    [SerializeField] private float slide_Speed;
    [SerializeField] private float slide_Time;
    [SerializeField] private float slide_PowerUp_Time;
    [SerializeField] private float slide_PowerUp_TimeCounter;
    private float slide_Timer_Counter;
    private bool is_Sliding;


    [Header("Collision Info")]
    [SerializeField] private float groundCheck;       // Distance to check for ground collision
    [SerializeField] private LayerMask whatIsGround;  // LayerMask defining what is considered "ground"
    [SerializeField] private Transform wall_Check;
    [SerializeField] private Vector2 wallCheckSize;
    private bool isGrounded;                          // Checks if the player is on the ground
    private bool wall_Detected;

    void Start()
    {
        // Initialize components
        _rigidbody2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Update the animator parameters based on player states
        Animator_Controller();


        slide_Timer_Counter = slide_Timer_Counter - Time.deltaTime;

        slide_PowerUp_TimeCounter = slide_PowerUp_TimeCounter - Time.deltaTime;

        // If movement is unlocked, move the player at the specified speed
        if (Player_Unlock && !wall_Detected)
        {
            Movement();
        }

        if (isGrounded)
        {
            can_Double_Jump = true; // Allow double jump after the initial jump
        }

        // Check if the player is grounded and manage jumping logic
        CheckCollision();

        CheckForSliding();

        // Check player input for specific actions
        CheckInput();
    }

    private void CheckForSliding()
    {
        if(slide_Timer_Counter < 0)
        {
            is_Sliding = false;
        }
    }

    private void Movement()
    {
        if(is_Sliding)
        {
            _rigidbody2D.velocity = new Vector2(slide_Speed, _rigidbody2D.velocity.y);
        }
        _rigidbody2D.velocity = new Vector2(moveSpeed, _rigidbody2D.velocity.y);
    }

    // Updates animator parameters to match the player’s state and velocity
    private void Animator_Controller()
    {
        isRunning_Anim_Bool = _rigidbody2D.velocity.x != 0; // Set running animation based on horizontal velocity

        // Update animator variables for each relevant state
        anim.SetFloat("Y_Velocity", _rigidbody2D.velocity.y); // Vertical velocity for jump/fall animations
        anim.SetFloat("X_Velocity", _rigidbody2D.velocity.x); // Horizontal velocity for movement animations
        
        anim.SetBool("canDoubleJump", can_Double_Jump);
        anim.SetBool("isRunning", isRunning_Anim_Bool);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isSliding", is_Sliding);
    }

    // Raycast to detect if the player is on the ground
    private void CheckCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheck, whatIsGround);
        wall_Detected = Physics2D.BoxCast(wall_Check.position, wallCheckSize, 0, Vector2.zero, 0, whatIsGround);
    }

    // Processes input for jump and unlocking movement
    private void CheckInput()
    {
        if (Input.GetButtonDown("Fire2")) // Unlocks movement when the Fire2 button is pressed
        {
            Player_Unlock = true;
        }

        if (Input.GetButtonDown("Jump")) // Initiates jump when Jump button is pressed
        {
            jump_Button();
        }

        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            SlideButton();       
        }
    }

    private void SlideButton()
    {
        if(_rigidbody2D.velocity.x != 0 && slide_PowerUp_TimeCounter < 0)
        {
            is_Sliding = true;
            slide_Timer_Counter = slide_Time;
            slide_PowerUp_TimeCounter = slide_PowerUp_Time;
        }
    }

    // Executes jump logic, including double jump capability
    private void jump_Button()
    {
        if (isGrounded) // If player is grounded, perform a normal jump
        {
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, jumpForce);
        }
        else if (can_Double_Jump) // If player is in the air and double jump is allowed
        {
            can_Double_Jump = false;      // Disable double jump after usage
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, doubleJumpForce); // Apply double jump force
        }
    }

    // Visual aid to show the ground-check ray in the Scene view for debugging
    public void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheck));
        Gizmos.DrawWireCube(wall_Check.position, wallCheckSize);
    }
}