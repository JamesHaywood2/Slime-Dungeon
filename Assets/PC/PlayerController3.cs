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
    

    [Header("Movement settings")]
    public float walkSpeed = 5f;
    private float direction = 0f;
    private float facing = 1;

    [Header("Jump Variables")]
    public int maxJumps;
    public int jumpCount;
    public float jumpForce = 10f;
    [SerializeField]private float maxFallSpeed = 15f;
    [SerializeField]private float coyoteTime = 0.2f; //Coyote time is what lets the player jump even when they were slightly off the platform.
    private float coyoteTimeCounter;
    [SerializeField]private float jumpBufferTime = 0.2f; //If you press the jump button, but you're not on the ground yet (still falling) you can still jump if it's within this time.
    private float jumpBufferCounter;
    private bool hasJumped;

    [Header("Wall Jump")]
    [SerializeField]private float wallSlideSpeed = 2f;
    [SerializeField]private Vector2 wallJumpPower = new Vector2(4f, 4f);
    [SerializeField]private float wallJumpDuration = 0.4f;
    private float wallJumpCounter;
    private bool isWallSliding;
    private bool isWallJumping;

    [Header("Dash variables")]
    [SerializeField]private float dashSpeed = 10f;
    [SerializeField]private float dashTime = 0.2f;
    [SerializeField]private float dashCooldown = 0.5f;
    private float dashCounter;
    private float dashCooldownCounter;
    private bool isDashing;


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
    public BoxCollider2D hitboxCollider;

    [Header("attack Cooldown")]
    [SerializeField]private float attackCooldown = 0.15f;
    private float attackCounter;

    [Header("Take damage variables")]
    [SerializeField]private float iFrameDuration = 0.2f;
    private float iFrameCounter;
    private bool isInvincible;
    private bool controlsEnabled;


    // Start is called before the first frame update
    void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        PlayerInfo.pInfo.currentRoom = currentScene.name;
        
        player = GetComponent<Rigidbody2D>();
        scale = new Vector3(player.transform.localScale.x, player.transform.localScale.y, player.transform.localScale.z);

        groundCheck = transform.Find("GroundCheck");
        wallCheck = transform.Find("WallCheck");

        hitboxCollider = transform.Find("Hitbox").GetComponent<BoxCollider2D>();

        this.animator = GetComponent<Animator>();

        //Stuff from PlayerInfo
        maxJumps = PlayerInfo.pInfo.allowedJumps;
        jumpCount = 0;
        controlsEnabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Check's if the player is entering left or right arrow indicating that they are moving.
        if (controlsEnabled){
            direction = Input.GetAxisRaw("Horizontal");
            if (direction != 0){
                facing = direction;
            }
        } 

        //Calls is grounded to check if the player is on the ground.
        //When on the ground the jump count is set to 0 and the coyote time counter is set to the coyote time.
        if (isGrounded()){
            this.animator.SetBool("grounded", true);

            coyoteTimeCounter = coyoteTime;
            jumpCount = 0;
            //If you're on the ground then you can't have jumped.
            hasJumped = false;

            maxJumps = PlayerInfo.pInfo.allowedJumps;

        } else {
            this.animator.SetBool("grounded", false);
            coyoteTimeCounter -= Time.deltaTime;
        }

        //Calls isWalled() to check if the player is up against a wall.
        //This and isGrounded could be in Update() or FixedUpdate(). I'm not sure which is a better idea.
        if (isWalled()){
            //If the player is walled, has the ability, is holding a direction, is not on the ground, and controls are enabled, then the player is wallsiding.
            if (PlayerInfo.pInfo.hasWallJump && direction != 0 && !isGrounded() && controlsEnabled){
                isWallSliding = true;
                this.animator.SetBool("wallSliding", true);
            } else {
                isWallSliding = false;
                this.animator.SetBool("wallSliding", false);
            }
        } else {
            isWallSliding = false;
        }

        //When you press the space bar it will set the jump buffer time.
        if (Input.GetKeyDown(KeyCode.Space) && controlsEnabled){
            jumpBufferCounter = jumpBufferTime;
        } else {
            jumpBufferCounter -= Time.deltaTime;
        }
        //Then in fixedUpdate it will check if the jump buffer time is greater than 0. If so you can jump.
        if (isWallSliding){
            WallSlide();
        }

        //If the jumpBufferCounter is greater than 0 and controls are enabled the player will jump.
        //If the player is wallsliding then they walljump.
        if (jumpBufferCounter > 0f &&  controlsEnabled){
            Jump();
        } 


        //Calls Attack() which checks if the player presses the attack button.
        //Only performs the check if the player's controls are enabled.
        if (controlsEnabled && PlayerInfo.pInfo.hasMelee){
            Attack();
        }


        //If the attack is not on cooldown (so attackCounter is greater than 0) then you can't make another attack.
        //Once the attack is off cooldown then it sets the hitbox to Off just in case the hitbox is still on.
        if (attackCounter < 0){
            hitboxOff();
        } else {
            attackCounter -= Time.deltaTime;
        }

        //iFrame counter is the ammount of time the player is invincible for.
        if (iFrameCounter < 0){
            isInvincible = false;
            controlsEnabled = true;
        } else {
            iFrameCounter -= Time.deltaTime;
        }

        if (isWallJumping && !isGrounded()){
            wallJumpCounter -= Time.deltaTime;
            if (wallJumpCounter < 0f){
                isWallJumping = false;
            }
        } else {
            Run();
        }

        //If dash is not on cooldown, controls are enabled, has the dash ability, and they press the C key then they dash.
        if (dashCooldownCounter < 0 && controlsEnabled && PlayerInfo.pInfo.hasDash && Input.GetKeyDown(KeyCode.C)){
            isDashing = true;
            dashCounter = dashTime;
            controlsEnabled = false;
            SoundManager.instance.PlaySound("dash");
        }

        //If the player is dashing then the dash counter is reduced by time.
        if (isDashing){
            dashCounter -= Time.deltaTime;
            player.velocity = new Vector2(dashSpeed * facing, player.velocity.y);
            if (dashCounter < 0){
                isDashing = false;
                dashCooldownCounter = dashCooldown;
            }
        } else {
            dashCooldownCounter -= Time.deltaTime;
            controlsEnabled=true;
        }



        //Animation
        if (this.animator !=null){
            Animate();
        }
    }

    private void FixedUpdate() {

        //This makes it so the player falls faster the longer they're in the air, up to a certain speed.
        if (player.velocity.y < 0 && !isGrounded() && !isWallSliding){
            player.gravityScale = player.gravityScale * 1.005f;
        } else {
            player.gravityScale = 1;
        }
        player.velocity = new Vector2(player.velocity.x, Mathf.Max(player.velocity.y, -maxFallSpeed));

        

    }

    //run is just the run command. If you have a direction held then you move in that direction. Otherwise you dont.
    private void Run(){
        if (direction != 0f && controlsEnabled){
                transform.localScale = new Vector3(facing * scale.x, scale.y, scale.z);
                player.velocity = new Vector2(facing * walkSpeed, player.velocity.y);
            } else if (controlsEnabled) {
                player.velocity = new Vector2(0, player.velocity.y);
            }
    }

    private void Jump(){
        //Jump is called in fixed update only if the jumpBufferCounter is greater than 0.
        jumpBufferCounter=0;
        coyoteTimeCounter=0;

        //If you haven't jumped, but aren't on the ground, and you aren't wall sliding, then you can jump one less time.
        if (hasJumped == false && isGrounded() == false && isWallSliding == false){
            maxJumps--;
        }


        //If the player is wallSliding at the time of the jump, which they can only do if they have the walljump ability, then they walljump.
        if (isWallSliding){
            Debug.Log("Walljump!");
            facing = -direction;
            player.velocity = new Vector2(facing * wallJumpPower.x, wallJumpPower.y);
            transform.localScale = new Vector3(facing * scale.x, scale.y, scale.z);
            isWallJumping = true;
            wallJumpCounter = wallJumpDuration;
            
            hasJumped = true;
        } else if ( (jumpCount < maxJumps) ){
            Debug.Log("Jump!");
            //If the player is not wallsliding, then they jump normally.
            player.velocity = new Vector2(player.velocity.x, jumpForce);
            jumpCount++;

            hasJumped = true;
        }

    }

    private void WallSlide(){
        player.velocity = new Vector2(player.velocity.x, Mathf.Clamp(player.velocity.y, -wallSlideSpeed, float.MaxValue));
    }


    private void Attack(){
        //If you press the attack key, it will resize a little hitbox to be big, then thens shrink it after a certain time.
        //Actual hits are detected in the HitDetection script of the hitbox object.
        if (Input.GetKeyDown(KeyCode.X) && (attackCounter < 0))
        {
            SoundManager.instance.PlaySound("attack");
            //Debug.Log("Attack!");
            attackCounter = attackCooldown;
            //Once you press the attack key it resizes the hitbox so it can actually hit stuff.
            if (Input.GetKey(KeyCode.UpArrow)){
                Debug.Log("Attack goes up");
                //AttackUp();
                this.animator.SetTrigger("Attack_Up");
            } else if (Input.GetKey(KeyCode.DownArrow) && isGrounded()==false){
                Debug.Log("Attack goes down");
                //AttackDown();
                this.animator.SetTrigger("Attack_Down");
            } else {
                Debug.Log("Attack goes forward");
                //AttackForward();
                this.animator.SetTrigger("Attack_Ground");
            }
        }
    }

    private void AttackUp(){
        hitboxCollider.size = new Vector2(3f, 1f);
        hitboxCollider.offset = new Vector2(0.1f, 1.5f);
    }

    private void AttackDown(){
        hitboxCollider.size = new Vector2(3f, 1.2f);
        hitboxCollider.offset = new Vector2(0.1f, -1.4f);
    }

    private void AttackForward(){
        hitboxCollider.size = new Vector2(2f, 1.5f);
        hitboxCollider.offset = new Vector2(1.3f, 0);
    }

    private void hitboxOff(){
        hitboxCollider.size = new Vector2(0, 0);
        hitboxCollider.offset = new Vector2(0, 0);
    }

    private void Animate(){
        if (direction !=0 && isGrounded() && (Mathf.Abs(player.velocity.x) > 0.15f)){
                this.animator.SetFloat("WalkSpeed", Mathf.Abs(player.velocity.x)/2f);
                this.animator.SetTrigger("Walking");
            }

        if (direction == 0 && isGrounded() && (Mathf.Abs(player.velocity.y)<0.15f)){
            this.animator.SetTrigger("Idle");
        }

        if ((player.velocity.y > 0) && !isGrounded()){
            this.animator.SetTrigger("Jumping");
        }

        if ((player.velocity.y < 0) && !isGrounded()){
            this.animator.SetTrigger("Falling");
        }

        if ((player.velocity.y < 0) && !isGrounded() && isWallSliding){
            this.animator.SetBool("wallSliding", true);
        } else {
            this.animator.SetBool("wallSliding", false);
        }

        //Add a damage animation here.
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

        }
        else if(other.tag == "Hazard")
        {
            player.transform.position = new Vector3(checkPoint.position.x, checkPoint.position.y, checkPoint.position.z);
            takeDamage(1);
        } 
        else if (other.tag == "Checkpoint")
        {
            Debug.Log("Checkpoint Updated");
            checkPoint = other.transform;
        } 
        else if (other.tag == "RespawnPoint")
        {
            respawnPoint = other.transform;
        } 
        //If the player's hurtbox collides with the 3's main collider, or the enemy's hitbox, then the player takes damage.
        //It will check if the player is invincible (invincible meaning they have taken damage recently).
        //If they are not invincible then it disables controls, sets the player to invincible, and then starts the invincibility timer.
        else if (other.tag == "Enemy" || other.tag == "HitBox") 
        {
            if (isInvincible == false){
                controlsEnabled = false;
                isInvincible = true;
                iFrameCounter = iFrameDuration;
                //Code for when player runs into an enemy.
                if (other.tag == "HitBox"){
                    Enemy enemy = other.GetComponentInParent<Enemy>();
                    takeDamage(enemy.attackDamage);
                    Debug.Log("Player got hit");
                } else {
                    takeDamage(1);
                }

                //direciton of the enemy relative to the player.
                facing = Mathf.Sign(other.transform.position.x - PlayerInfo.pInfo.playerPos.x);
                Debug.Log("Direction: " + facing);
                //Sets the player's velocity to zero and then adds a knockback force.
                //The second addforce is to make sure the player jumps up a bit.
                player.velocity = new Vector2(0, 0);
                player.AddForce(new Vector2(-facing*250, 150));
            } else {
                Debug.Log("Player is invincible");
            }
        } else if (other.tag == "UnlockTrigger") //If the player touches a one way door (which has the unlock trigger tag) delete the door.
        {
            string doorName = PlayerInfo.pInfo.currentRoom + "*"+ other.transform.parent.name;
            PlayerInfo.pInfo.oneWayDoors.Add(doorName, true);
            Destroy(other.transform.parent.gameObject);
        }
        else if (other.tag == "meleeItem")
        {
            Destroy(other.gameObject);
            PlayerInfo.pInfo.hasMelee = true;
        }else if (other.tag == "meleeUpgradeItem"){
            Destroy(other.gameObject);
            PlayerInfo.pInfo.attackDamage += 2;
            PlayerInfo.pInfo.hasMeleeUpgrade = true;
        } 
        else if (other.tag == "wallBreakItem")
        {
            Destroy(other.gameObject);
            PlayerInfo.pInfo.hasWallBreak = true;
        } else if (other.tag == "doubleJumpItem")
        {
            Destroy(other.gameObject);
            PlayerInfo.pInfo.allowedJumps = 2;
            PlayerInfo.pInfo.hasDoubleJump = true;
            maxJumps = 2;
        } else if (other.tag == "wallJumpItem")
        {
            Destroy(other.gameObject);
            PlayerInfo.pInfo.hasWallJump = true;
        } else if (other.tag == "dashItem")
        {
            Destroy(other.gameObject);
            PlayerInfo.pInfo.hasDash = true;
        } else if (other.tag == "warpItem"){
            Destroy(other.gameObject);
            PlayerInfo.pInfo.hasWarp = true;
        }
    }

    private bool isGrounded(){
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private bool isWalled(){
        return Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, wallLayer);
    }
    

    //Late update, if I understand it correctly, is called after update is done.
    private void LateUpdate(){
        PlayerInfo.pInfo.playerPos = new Vector2(player.position.x, player.position.y);
    }


    public void takeDamage(int damage){
        SoundManager.instance.PlaySound("playerHit");
        //Updates health variable.
        PlayerInfo.pInfo.currentHealth -= damage;
        if (PlayerInfo.pInfo.currentHealth <= 0){
            //If the player's health reaches 0, then they die.
            //Just reset the player's health and reload the scene for now.
            PlayerInfo.pInfo.currentHealth = PlayerInfo.pInfo.maxHealth;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
