using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController3 : MonoBehaviour
{

    [Header("components")]
    LoadZoneInfo loadZone;

    Rigidbody2D player;
    private Vector3 scale;
    Animator animator;


    [Header("Misc Player Variables")]
    public int maxJumps;
    public int health;
    

    [Header("Movement settings")]
    private float direction = 0f;
    public float walkSpeed = 5f;
    public float jumpForce = 10f;
    public int availableJumps;
    private bool isWallSliding;
    [SerializeField]private float wallSlideSpeed = 2f;

    [SerializeField]private bool isWallJumping;
    private float wallJumpCounter;
    public float wallJumpDuration = 0.4f;
    [SerializeField]private Vector2 wallJumpPower = new Vector2(4f, 4f);
    

    [SerializeField]private float maxFallSpeed = 15f;

    //Coyote time is what lets the player jump even when they were slightly off the platform.
    [SerializeField]private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    [SerializeField]private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;



    [Header("Ground checking variables")]
    public bool onGround;
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundLayer;
    private bool hasJumped;

    [Header("Wall checking varibales")]
    public bool onWall;
    public Transform wallCheck;
    public float wallCheckRadius;
    public LayerMask wallLayer;


    [Header("Respawn/Checkpoints")]
    private Transform checkPoint;
    private Transform respawnPoint;


    [Header("Hitbox")]
    public GameObject hitbox;
    private BoxCollider2D hitboxCollider;
    [SerializeField]private float hitTime = 0.2f;
    private float hitCounter;

    [Header("Hit Cooldown")]
    public float attackCooldown = 0.15f;
    private float attackCounter;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Rigidbody2D>();
        scale = new Vector3(player.transform.localScale.x, player.transform.localScale.y, player.transform.localScale.z);

        hitboxCollider = hitbox.GetComponent<BoxCollider2D>();

        this.animator = GetComponent<Animator>();

        //Stuff from PlayerInfo
        maxJumps = PlayerInfo.pInfo.allowedJumps;
        availableJumps = maxJumps;
        health = PlayerInfo.pInfo.currentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        //Check's if the player is entering left or right arrow indicating that they are moving.
        direction = Input.GetAxisRaw("Horizontal");

        //Calls isWalled() to check if the player is up against a wall.
        //This and isGrounded could be in Update() or FixedUpdate(). I'm not sure which is a better idea.
        if (isWalled()){
            onWall = true;
        } else {
            onWall = false;
        }

        //Calls isWalled() to check if the player is up against a wall.
        //This and isWalled could be in Update() or FixedUpdate(). I'm not sure which is a better idea.
        if (isGrounded()){
            onGround = true;
        } else {
            onGround = false;
        }

        //If the player is on a wall, in the air, holding a direction, and has the ability unlocked, then it sets isWallSliding indicates to the system that the player is wallSliding.
        if (PlayerInfo.pInfo.hasWallJump && onWall && !onGround && direction!=0){
            isWallSliding = true;
            this.animator.SetBool("wallSliding", true);
        } else {
            isWallSliding = false;
            this.animator.SetBool("wallSliding", false);
        }
        
        //If the player is on the ground they haven't jumped.
        if (onGround){
            this.animator.SetBool("grounded", true);

            coyoteTimeCounter = coyoteTime;

            availableJumps = maxJumps;
            hasJumped = false;

        } else {
            this.animator.SetBool("grounded", false);
            coyoteTimeCounter -= Time.deltaTime;
        }

         //If you are not on the ground and have not jumped then you have walked off a ledge meaning you will have one less jump available.
        if ((coyoteTimeCounter < 0f) && isGrounded() == false && hasJumped == false){
            availableJumps = maxJumps-1;
        }

        //If you press jump it will set the jump buffer counter to the jump buffer time. It will then immeditally start counting down.
        if (Input.GetButtonDown("Jump")){
            jumpBufferCounter = jumpBufferTime;
        } else {
            jumpBufferCounter -= Time.deltaTime;
        }


        //If you press the attack key, it will resize a little hitbox to be big, then thens shrink it after a certain time.
        //Actual hits are detected in the HitDetection script of the hitbox object.
        if (PlayerInfo.pInfo.hasMelee &&Input.GetKeyDown(KeyCode.X) && (attackCounter < 0)){
            //Debug.Log("Attack!");

            hitCounter = hitTime;
            attackCounter = attackCooldown;
            
            //Once you press the attack key it resizes the hitbox so it can actually hit stuff.
            if (Input.GetKey(KeyCode.UpArrow)){
                Debug.Log("Attack goes up");
                hitboxCollider.size = new Vector2(3f, 1f);
                hitboxCollider.offset = new Vector2(0.1f, 1.5f);
                this.animator.SetTrigger("Attack_Up");
            } else if (Input.GetKey(KeyCode.DownArrow) && isGrounded()==false){
                Debug.Log("Attack goes down");
                hitboxCollider.size = new Vector2(3f, 1.2f);
                hitboxCollider.offset = new Vector2(0.1f, -1.4f);
                this.animator.SetTrigger("Attack_Down");
            } else {
                Debug.Log("Attack goes forward");
                hitboxCollider.size = new Vector2(2f, 1.5f);
                hitboxCollider.offset = new Vector2(1.3f, 0);
                this.animator.SetTrigger("Attack_Ground");
            }
            
        } else {
            hitCounter -= Time.deltaTime;
            attackCounter -= Time.deltaTime;
        }

        if (hitCounter < 0){
            hitboxCollider.size = new Vector2(0, 0);
            hitboxCollider.offset = new Vector2(0, 0);
        }

        //Animation
        if (this.animator !=null){
            if (direction !=0 && onGround && (Mathf.Abs(player.velocity.x) > 0.15f)){
                this.animator.speed = walkSpeed / 2.0f;
                this.animator.SetTrigger("Walking");
            }

            if (direction == 0 && onGround && (Mathf.Abs(player.velocity.y)<0.15f)){
                this.animator.speed=1f;
                this.animator.SetTrigger("Idle");
            }

            if ((player.velocity.y > 0) && !onGround){
                this.animator.speed=1f;
                this.animator.SetTrigger("Jumping");
            }

            if ((player.velocity.y < 0) && !onGround){
                this.animator.speed=1f;
                this.animator.SetTrigger("Falling");
            }

            if ((player.velocity.y < 0) && !onGround && isWallSliding){
                this.animator.SetBool("wallSliding", true);
            } else {
                this.animator.SetBool("wallSliding", false);
            }
        }

    }

    private void FixedUpdate() {

        //If the player is wallsliding then it will clamp their velocity.
        if (isWallSliding){
            player.velocity = new Vector2(player.velocity.x, Mathf.Clamp(player.velocity.y, -wallSlideSpeed, float.MaxValue));
        }

        //This makes it so the player falls faster the longer they're in the air, up to a certain speed.
        if (player.velocity.y < 0 && !onGround && !isWallSliding){
            player.gravityScale = player.gravityScale * 1.005f;
        } else {
            player.gravityScale = 1;
        }
        player.velocity = new Vector2(player.velocity.x, Mathf.Max(player.velocity.y, -maxFallSpeed));

        //If jumpBufferCounter is greater than 0, which it will be the frame you press space, and you have available jumps you then jump.
        //Additionally is you are sliding on a wall, and jumpBuffer>0 then you have wall jumped.
        if ((isWallSliding) && (jumpBufferCounter > 0f)){
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
            hasJumped = true;

            isWallJumping = true;
            wallJumpCounter = wallJumpDuration;
            player.velocity = new Vector2(-direction * wallJumpPower.x, wallJumpPower.y);
        } 
        else if ((jumpBufferCounter > 0f) && availableJumps > 0){
            player.velocity = new Vector2(player.velocity.x, jumpForce);
            availableJumps--;
            hasJumped = true;

            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        } 

        //Above there is a section that determines if you have wall jumped.
        //For the duration of the wall jump you don't actually have control over your character.
        //If you're in the wall jump then you have control like normal.
        if (isWallJumping && !onGround){
            wallJumpCounter -= Time.deltaTime;
            if (wallJumpCounter < 0f){
                isWallJumping = false;
            }
        } else {
            run();
        }

    }

    //run is just the run command. If you have a direction held then you move in that direction. Otherwise you dont.
    private void run(){
        if (direction != 0f){
                transform.localScale = new Vector3(direction * scale.x, scale.y, scale.z);
                player.velocity = new Vector2(direction * walkSpeed, player.velocity.y);
            } else {
                player.velocity = new Vector2(0, player.velocity.y);
            }
    }


    private void OnTriggerEnter2D(Collider2D other) {
        //Debug.Log("Player collided with " + other.name);
        if (other.tag == "LoadZone")
        {
            //the player walks into a collision zone tagged LoadZone it will then go and find the GameObject of the loadzone.
            //The gameobject has the destination as a public field.
            //It then loads that scene
            Debug.Log("Entered Loadzone: " + other.name);
            GameObject LZ = GameObject.Find(other.name);
            loadZone = LZ.GetComponent<LoadZoneInfo>();
            
            PlayerInfo.pInfo.lastRoom = LZ.scene.name;

            SceneManager.LoadScene(loadZone.Destination);

        } else if (other.tag == "AggroRange"){
            Enemy enemy = other.GetComponentInParent<Enemy>();
            enemy.isAggro = true;
        }
        else if(other.tag == "Hazard")
        {
            player.transform.position = new Vector3(checkPoint.position.x, checkPoint.position.y, checkPoint.position.z);
        } 
        else if (other.tag == "Checkpoint")
        {
            Debug.Log("Checkpoint Updated");
            checkPoint = other.transform;
        } 
        else if (other.tag == "RespawnPoint")
        {
            respawnPoint = other.transform;
        } else if (other.tag == "Enemy")
        {
            //Code for when hit by an enemy.
        } else if (other.tag == "meleeItem")
        {
            Destroy(other.gameObject);
            PlayerInfo.pInfo.hasMelee = true;
        } else if (other.tag == "wallBreakItem")
        {
            Destroy(other.gameObject);
            PlayerInfo.pInfo.hasWallBreak = true;
        } else if (other.tag == "doubleJumpItem")
        {
            Destroy(other.gameObject);
            PlayerInfo.pInfo.allowedJumps = 2;
            maxJumps = 2;
        } else if (other.tag == "wallJumpItem")
        {
            Destroy(other.gameObject);
            PlayerInfo.pInfo.hasWallJump = true;
        } else if (other.tag == "dashItem")
        {
            Destroy(other.gameObject);
            PlayerInfo.pInfo.hasDash = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "FOV" && other.transform.parent.gameObject.tag == "Enemy"){
            Enemy enemy = other.transform.parent.gameObject.GetComponent<Enemy>();
            enemy.isAggro = false;
            enemy.isReturning = true;
        }
    }


    private bool isGrounded(){
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private bool isWalled(){
        return Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, wallLayer);
    }

    private void WallSlide(){
        if (PlayerInfo.pInfo.hasWallJump && onWall && !onWall && direction!=0){
            Debug.Log("calling wallSlide");
            isWallSliding = true;
            player.velocity = new Vector2(player.velocity.x, Mathf.Clamp(player.velocity.y, -wallSlideSpeed, float.MaxValue));
            this.animator.SetBool("wallSliding", true);
        } else {
            isWallSliding = false;
            this.animator.SetBool("wallSliding", false);
        }
    }
    

    //Late update, if I understand it correctly, is called after update is done.
    private void LateUpdate(){
        PlayerInfo.pInfo.currentHealth = health;
        PlayerInfo.pInfo.playerPos = new Vector2(player.position.x, player.position.y);
    }

}
