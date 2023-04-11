using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Enumeration for attack types.
public enum AttackType
{
    Default,
    Up,
    Down,
}

public class movementTest : MonoBehaviour
{
    [Header("components")]
    LoadZoneInfo loadZone;
    Rigidbody2D player;
    private Vector3 scale;
    Animator animator;


    [Header("Movement states")]
    public bool controlsEnabled;
    public bool isRunning;
    public bool isJumping;
    public bool isFalling;
    public bool isAttacking;
    public bool isDamaged;
    public bool isDead;
    public bool isGrounded;
    public bool isWalled;
    public bool isSliding;
    public bool isInvincible;


    [Header("Movement settings")]
    public float maxWalkSpeed = 5f;
    public float walkAcceleration = 5f;
    private float walkDirection = 1f;
    private float direction = 1f;


    [Header("Jump Variables")]
    public int maxJumps;
    public int jumpCount;
    public float jumpForce = 10f;
    [SerializeField]private float maxFallSpeed = 15f;
    [SerializeField]private float coyoteTime = 0.2f; //Coyote time is what lets the player jump even when they were slightly off the platform.
    private float coyoteTimeCounter;
    [SerializeField]private float jumpBufferTime = 0.2f; //If you press the jump button, but you're not on the ground yet (still falling) you can still jump if it's within this time.
    private float jumpBufferCounter;
    [SerializeField]private float jumpCooldownTime;
    private float jumpCooldownCounter;

    [Header("Wall Jump")]
    [SerializeField]private float wallSlideSpeed = 2f;
    [SerializeField]private Vector2 wallJumpPower = new Vector2(4f, 4f);
    [SerializeField]private float wallJumpDuration = 0.4f;
    private float wallJumpCounter;

    [Header("Ground checking variables")]
    [SerializeField]private Transform groundCheck;
    [SerializeField]private float groundCheckRadius;
    [SerializeField]private LayerMask groundLayer;

    [Header("Wall checking varibales")]
    [SerializeField]private Transform wallCheck;
    [SerializeField]private float wallCheckRadius;
    [SerializeField]private LayerMask wallLayer;

    [Header("Respawn/Checkpoints")]
    private Transform checkPoint;
    private Transform respawnPoint;

    [Header("Hitbox")]
    [SerializeField]private BoxCollider2D hitboxCollider;


    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Rigidbody2D>();
        scale = new Vector3(player.transform.localScale.x, player.transform.localScale.y, player.transform.localScale.z);

        groundCheck = transform.Find("GroundCheck");
        wallCheck = transform.Find("WallCheck");
        hitboxCollider = transform.Find("Hitbox").GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

        //Stuff from PlayerInfo
        maxJumps = PlayerInfo.pInfo.allowedJumps;
        jumpCount = 0;
        controlsEnabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        #region Timers
        coyoteTimeCounter -= Time.deltaTime;
        jumpBufferCounter -= Time.deltaTime;
        wallJumpCounter -= Time.deltaTime;
        #endregion



        //Check if the player is grounded or on a wall.
        isGrounded = IsGrounded();
        isWalled = IsWalled();

        //If the player is grounded, reset jump count and coyote time.
        if (isGrounded){
            jumpCount = 0;
            isJumping = false;
            coyoteTimeCounter = coyoteTime;
        } else {
            //If the player is not grounded, and they velocity is negative, then they are falling.
            if (player.velocity.y <0f){
                isFalling = true;
            } else {
                isFalling = false;
            }
            
            //If the player is not on the ground, have not previously jumped, and coyote time is not valid, then lose a jump.
            if (jumpCount == 0 && coyoteTimeCounter <= 0){
                jumpCount++;
            }
        }

        //Get input for controls
        if (controlsEnabled){
            //If the player presses the space bar then they are jumping.
            if (Input.GetKeyDown(KeyCode.Space)){
                jumpBufferCounter = jumpBufferTime;
            }
            //Basically if the player has pressed the jump button, but they haven't landed yet, they can still jump.
            if (jumpBufferCounter > 0f){
                isJumping = true;
                jumpBufferCounter = 0f;
                coyoteTimeCounter = 0f;
                Jump();
            }


            //If the player presses the left or right arrow keys then they are moving.
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)){
                isRunning = true;
                walkDirection = Input.GetAxisRaw("Horizontal");
                direction = walkDirection;
            }
            else{
                isRunning = false;
                walkDirection = 0f;
            }

            //If the player presses the X key then they launch an attack.
            // if (Input.GetKeyDown(KeyCode.X)){
            //     isAttacking = true;
            //     if (Input.GetKey(KeyCode.UpArrow)){
            //         Attack(AttackType.Up);
            //     }
            //     else if (Input.GetKey(KeyCode.DownArrow)){
            //         Attack(AttackType.Down);
            //     }
            //     else{
            //         Attack(AttackType.Default);
            //     }
            // }
        } else {
            isRunning = false;
            isJumping = false;
            isAttacking = false;
        }




    
        //Update the direciton the player is facing.
        transform.localScale = new Vector3(scale.x * direction, scale.y, scale.z);
    }

    private void FixedUpdate() {
        if (controlsEnabled){
            Run();
        }
    }

    private void Run(){
        float targetSpeed = walkDirection * maxWalkSpeed;
        //We can reduce are control using Lerp() this smooths changes to are direction and speed
		targetSpeed = Mathf.Lerp(player.velocity.x, targetSpeed, 1);

        //Calculate difference between current velocity and desired velocity
		float speedDif = targetSpeed - player.velocity.x;

		//Calculate force along x-axis to apply to thr player
		float movement = speedDif * walkAcceleration;

		//Convert this to a vector and apply to rigidbody
		player.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    private void Jump(){
        //If you're on the ground or in coyote time you can do a normal jump.
        if (isGrounded || coyoteTimeCounter > 0){
            player.velocity = new Vector2(player.velocity.x, jumpForce);
            jumpCount++;
            coyoteTimeCounter = 0;
        }
        else if (jumpCount < maxJumps){
            player.velocity = new Vector2(player.velocity.x, jumpForce);
            jumpCount++;
        }
    }

    private void WallJump(){

    }

    private void Attack(AttackType attackType){
        
        switch (attackType){
            case AttackType.Default:
                animator.SetTrigger("Attack");
                break;
            case AttackType.Up:
                animator.SetTrigger("AttackUp");
                break;
            case AttackType.Down:
                animator.SetTrigger("AttackDown");
                break;
        }

    }

    private bool IsGrounded(){
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private bool IsWalled(){
        return Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, wallLayer);
    }

}
