using System;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Components
    private Rigidbody2D _rigidbody2D;  // Reference to Rigidbody2D for physics-based movement
    private Animator anim;            // Reference to Animator for controlling animations

    // Animation Parameters
    [SerializeField] private bool isRunning_Anim_Bool;

    // Movement Parameters
    [Header("Move Info")]
    [SerializeField] private float moveSpeed;        // Speed for horizontal movement
    [SerializeField] private float jumpForce;        // Force for initial jump
    [SerializeField] private float doubleJumpForce;  // Force for double jump
    private bool can_Double_Jump;                    // Tracks double-jump availability
    [SerializeField] private bool Player_Unlock = false; // Unlocks player movement upon input

    // Sliding Parameters
    [Header("Slide Info")]
    [SerializeField] private float slide_Speed;       // Sliding speed
    [SerializeField] private float slide_Time;        // Duration of slide
    [SerializeField] private float slide_PowerUp_Time; // Cooldown time for next slide
    private float slide_PowerUp_TimeCounter;         // Timer for slide cooldown
    private float slide_Timer_Counter;               // Timer for active slide
    private bool is_Sliding;                         // Tracks if the player is sliding

    // Collision Detection Parameters
    [Header("Collision Info")]
    [SerializeField] private float groundCheck;       // Distance for ground detection
    [SerializeField] private float sealingCheck;      // Distance for ceiling detection
    [SerializeField] private LayerMask whatIsGround;  // LayerMask defining ground
    [SerializeField] private Transform wall_Check;    // Position for wall detection
    [SerializeField] private Vector2 wallCheckSize;   // Size for wall detection area
    private bool isGrounded;                          // Tracks if the player is on the ground
    private bool isSliding_RC;                        // Tracks if something blocks sliding
    private bool wall_Detected;                       // Tracks if a wall is detected

    public bool ledge_Detection;

    private void Start()
    {
        // Initialize components
        _rigidbody2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // Update collision states
        CheckCollision();

        // Update animator parameters
        UpdateAnimator();

        // Decrease slide-related timers
        slide_Timer_Counter -= Time.deltaTime;
        slide_PowerUp_TimeCounter -= Time.deltaTime;

        // Manage sliding state
        CheckForSliding();

        // Handle movement and input if movement is unlocked
        if (Player_Unlock)
        {
            Movement();
        }

        // Reset double-jump ability when grounded
        if (isGrounded)
        {
            can_Double_Jump = true;
        }

        // Handle user input for actions
        HandleInput();
    }

    /// <summary>
    /// Handles user input for movement and actions.
    /// </summary>
    private void HandleInput()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            // Unlock player movement
            Player_Unlock = true;
        }

        if (Input.GetButtonDown("Jump"))
        {
            // Handle jump action
            HandleJump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            // Handle slide action
            InitiateSlide();
        }
    }

    /// <summary>
    /// Controls player movement, including normal movement and sliding.
    /// </summary>
    private void Movement()
    {
        // Prevent movement if a wall is detected
        if (wall_Detected) return;

        if (is_Sliding)
        {
            // Apply sliding velocity
            _rigidbody2D.velocity = new Vector2(slide_Speed, _rigidbody2D.velocity.y);
        }
        else
        {
            // Apply normal movement velocity
            _rigidbody2D.velocity = new Vector2(moveSpeed, _rigidbody2D.velocity.y);
        }
    }

    /// <summary>
    /// Checks for and manages sliding state.
    /// </summary>
    private void CheckForSliding()
    {
        if (slide_Timer_Counter < 0 && !isSliding_RC)
        {
            is_Sliding = false;
        }
    }

    /// <summary>
    /// Handles the slide initiation logic.
    /// </summary>
    private void InitiateSlide()
    {
        if (_rigidbody2D.velocity.x != 0 && slide_PowerUp_TimeCounter < 0)
        {
            is_Sliding = true;
            slide_Timer_Counter = slide_Time;
            slide_PowerUp_TimeCounter = slide_PowerUp_Time;
        }
    }

    /// <summary>
    /// Handles the jumping logic, including double-jump capability.
    /// </summary>
    private void HandleJump()
    {
        // Prevent jumping during a slide
        if (is_Sliding) return;

        if (isGrounded)
        {
            // Perform initial jump
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, jumpForce);
        }
        else if (can_Double_Jump)
        {
            // Perform double jump
            can_Double_Jump = false;
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, doubleJumpForce);
        }
    }

    /// <summary>
    /// Updates animator parameters to match the player's state.
    /// </summary>
    private void UpdateAnimator()
    {
        // Update animation parameters based on player's velocity and state
        isRunning_Anim_Bool = _rigidbody2D.velocity.x != 0;
        anim.SetFloat("Y_Velocity", _rigidbody2D.velocity.y);
        anim.SetFloat("X_Velocity", _rigidbody2D.velocity.x);
        anim.SetBool("canDoubleJump", can_Double_Jump);
        anim.SetBool("isRunning", isRunning_Anim_Bool);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isSliding", is_Sliding);
    }

    /// <summary>
    /// Checks for collision states such as ground, wall, and ceiling detection.
    /// </summary>
    private void CheckCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheck, whatIsGround);
        isSliding_RC = Physics2D.Raycast(transform.position, Vector2.up, sealingCheck, whatIsGround);
        wall_Detected = Physics2D.BoxCast(wall_Check.position, wallCheckSize, 0, Vector2.zero, 0, whatIsGround);

        Debug.Log(ledge_Detection);
    }

    /// <summary>
    /// Draws collision detection gizmos for debugging in the Scene view.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheck));

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y + sealingCheck));

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wall_Check.position, wallCheckSize);
    }
}