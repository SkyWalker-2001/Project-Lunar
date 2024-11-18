using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    // References to components
    private Rigidbody2D _rigidbody2D;  // Physics-based movement handler
    private Animator anim;            // Controls player animations

    // Animation Parameters
    [SerializeField, Tooltip("Tracks if the player is running (used for animations).")]
    private bool isRunning_Anim_Bool;

    [Header("Speed Info")]
    [SerializeField, Tooltip("Maximum speed the player can reach.")]
    private float max_Speed;

    [SerializeField, Tooltip("Factor to multiply the speed when a milestone is reached.")]
    private float speed_Multiplier;

    private float defaultSpeed; // Stores the original speed for resetting

    [Space]
    [SerializeField, Tooltip("Distance threshold after which the player's speed increases.")]
    private float milestone_Inceaser;

    private float defaultMilestoneIncreaser; // Stores the original milestone distance for resetting
    private float speed_Milestone;          // Tracks the current milestone distance

    [Header("Move Info")]
    [SerializeField, Tooltip("Initial movement speed of the player.")]
    private float moveSpeed;

    [SerializeField, Tooltip("Force applied when the player jumps.")]
    private float jumpForce;

    [SerializeField, Tooltip("Force applied for double jumps.")]
    private float doubleJumpForce;

    private bool can_Double_Jump; // Tracks if the player can perform a double jump

    [SerializeField, Tooltip("Determines if the player can move (unlocked after an event).")]
    private bool Player_Unlock;

    [Header("Slide Info")]
    [SerializeField, Tooltip("Speed of the player while sliding.")]
    private float slide_Speed;

    [SerializeField, Tooltip("Duration for which the slide lasts.")]
    private float slide_Time;

    [SerializeField, Tooltip("Cooldown time before the player can slide again.")]
    private float slide_PowerUp_Time;

    private float slide_PowerUp_TimeCounter; // Timer to track slide cooldown
    private float slide_Timer_Counter;       // Timer for active slide duration
    private bool is_Sliding;                 // Tracks if the player is sliding

    [Header("Collision Info")]
    [SerializeField, Tooltip("Distance to check if the player is grounded.")]
    private float groundCheck;

    [SerializeField, Tooltip("Distance to check if there is an obstacle above the player.")]
    private float sealingCheck;

    [SerializeField, Tooltip("Defines what layers are considered 'ground'.")]
    private LayerMask whatIsGround;

    [SerializeField, Tooltip("Transform used to check for walls in the player's path.")]
    private Transform wall_Check;

    [SerializeField, Tooltip("Size of the area used for wall detection.")]
    private Vector2 wallCheckSize;

    private bool isGrounded;    // Indicates if the player is on the ground
    private bool isSliding_RC;  // Tracks if something blocks sliding
    private bool wall_Detected; // Indicates if the player is near a wall

    [HideInInspector]
    public bool ledge_Detection; // Tracks if a ledge is detected (used for ledge mechanics)

    [Header("Ledge Info")]
    [SerializeField, Tooltip("Offset for the position before climbing a ledge.")]
    private Vector2 offset1;

    [SerializeField, Tooltip("Offset for the position after climbing a ledge.")]
    private Vector2 offset2;

    private Vector2 climb_Begin_Position; // Position to start climbing
    private Vector2 climb_Over_Position; // Position after climbing
    private bool can_Grab_Ledge = true;  // Determines if the player can grab ledges
    private bool can_Climb;              // Determines if the player can climb a ledge

    private void Start()
    {
        // Get references to required components
        _rigidbody2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Initialize speed and milestone values
        speed_Milestone = milestone_Inceaser;
        defaultSpeed = moveSpeed;
        defaultMilestoneIncreaser = milestone_Inceaser;
    }

    private void Update()
    {
        // Check for collisions with the ground, walls, and other elements
        CheckCollision();

        // Sync player state with animations
        UpdateAnimator();

        // Update slide cooldown timers
        slide_Timer_Counter -= Time.deltaTime;
        slide_PowerUp_TimeCounter -= Time.deltaTime;

        // Manage sliding mechanics
        CheckForSliding();

        // Handle ledge interaction mechanics
        CheckForLedge();

        // Control speed progression based on milestones
        SpeedController();

        // Allow movement only if the player is unlocked
        if (Player_Unlock)
        {
            Movement();
        }

        // Reset double-jump ability when grounded
        if (isGrounded)
        {
            can_Double_Jump = true;
        }

        // Process player input for actions like jumping and sliding
        HandleInput();
    }

    #region SpeedController
    /// <summary>
    /// Resets the player's speed and milestone settings to their default values.
    /// </summary>
    private void SpeedReset()
    {
        moveSpeed = defaultSpeed;
        milestone_Inceaser = defaultMilestoneIncreaser;
    }

    /// <summary>
    /// Increases the player's speed when they reach a milestone distance.
    /// </summary>
    private void SpeedController()
    {
        // If the player has already reached max speed, do nothing
        if (moveSpeed == max_Speed)
        {
            return;
        }

        // Check if the player has crossed the next milestone
        if (transform.position.x > speed_Milestone)
        {
            // Update the next milestone and increase speed
            speed_Milestone = speed_Milestone + milestone_Inceaser;
            moveSpeed = moveSpeed * speed_Multiplier;
            milestone_Inceaser = milestone_Inceaser * speed_Multiplier;

            // Clamp the speed to avoid exceeding max speed
            if (moveSpeed > max_Speed)
            {
                moveSpeed = max_Speed;
            }
        }
    }
    #endregion

    /// <summary>
    /// Handles player movement, including normal movement and sliding.
    /// Resets speed when a wall is detected.
    /// </summary>
    private void Movement()
    {
        if (wall_Detected)
        {
            SpeedReset();
            return;
        }

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
    /// Detects ledges and sets the player's position to climb over.
    /// </summary>
    private void CheckForLedge()
    {
        if (ledge_Detection && can_Grab_Ledge)
        {
            can_Grab_Ledge = false;

            Vector2 ledgePosition = GetComponentInChildren<Ledge_Check>().transform.position;
            climb_Begin_Position = ledgePosition + offset1;
            climb_Over_Position = ledgePosition + offset2;

            can_Climb = true;
        }

        if (can_Climb)
        {
            transform.position = climb_Begin_Position;
        }
    }

    /// <summary>
    /// Handles player input for movement, jumping, and sliding.
    /// </summary>
    private void HandleInput()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            Player_Unlock = true;
        }

        if (Input.GetButtonDown("Jump"))
        {
            HandleJump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            InitiateSlide();
        }
    }

    private void HandleJump()
    {
        // Prevent jumping while sliding
        if (is_Sliding) return;

        if (isGrounded)
        {
            // Apply force for a normal jump
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, jumpForce);
        }
        else if (can_Double_Jump)
        {
            // Apply force for a double jump
            can_Double_Jump = false;
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, doubleJumpForce);
        }
    }

    private void CheckForSliding()
    {
        // Stop sliding if the timer runs out
        if (slide_Timer_Counter < 0 && !isSliding_RC)
        {
            is_Sliding = false;
        }
    }

    private void InitiateSlide()
    {
        // Start sliding if conditions are met
        if (_rigidbody2D.velocity.x != 0 && slide_PowerUp_TimeCounter < 0)
        {
            is_Sliding = true;
            slide_Timer_Counter = slide_Time;
            slide_PowerUp_TimeCounter = slide_PowerUp_Time;
        }
    }

    private void CheckCollision()
    {
        // Check for ground, ceiling, and walls
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheck, whatIsGround);
        isSliding_RC = Physics2D.Raycast(transform.position, Vector2.up, sealingCheck, whatIsGround);
        wall_Detected = Physics2D.BoxCast(wall_Check.position, wallCheckSize, 0, Vector2.zero, 0, whatIsGround);
    }

    private void UpdateAnimator()
    {
        // Update animation states
        isRunning_Anim_Bool = _rigidbody2D.velocity.x != 0;
        anim.SetFloat("Y_Velocity", _rigidbody2D.velocity.y);
        anim.SetFloat("X_Velocity", _rigidbody2D.velocity.x);
        anim.SetBool("canDoubleJump", can_Double_Jump);
        anim.SetBool("isRunning", isRunning_Anim_Bool);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isSliding", is_Sliding);
        anim.SetBool("canClimb", can_Climb);
    }

    private void OnDrawGizmos()
    {
        // Visualize collision detection areas in the Scene view
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheck));

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y + sealingCheck));

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wall_Check.position, wallCheckSize);
    }
}