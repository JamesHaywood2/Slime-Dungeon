using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController2 : MonoBehaviour
{
    [Header("components")]
    LoadZoneInfo loadZone;

    Rigidbody2D player;
    private Vector3 scale;
    Animator animator;
    

    [Header("Movement settings")]
    private float direction = 0f;
    public float walkSpeed = 5f;
    public float jumpForce = 10f;
    public int maxJumps;
    public int availableJumps;

    [SerializeField]private float maxFallSpeed = 15f;

    //Coyote time is what lets the player jump even when they were slightly off the platform.
    [SerializeField]private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    [SerializeField]private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;



    [Header("Ground checking variables")]
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundLayer;
    private bool hasJumped;

    [Header("Respawn/Checkpoints")]
    private Transform checkPoint;
    private Transform respawnPoint;

    [Header("Hitbox")]
    public GameObject hitbox;
    private BoxCollider2D hitboxCollider;
    [SerializeField]private float hitTime = 0.2f;
    public float hitCounter;


    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Rigidbody2D>();
        scale = new Vector3(player.transform.localScale.x, player.transform.localScale.y, player.transform.localScale.z);

        hitboxCollider = hitbox.GetComponent<BoxCollider2D>();

        this.animator = GetComponent<Animator>();

        maxJumps = PlayerInfo.pInfo.getAllowedJumps();
        availableJumps = maxJumps;
    }

    // Update is called once per frame
    void Update()
    {


        //JUMPING
        //If you are on the ground then you have all your jumps and haven't jumped.
        if (isGrounded()){

            coyoteTimeCounter = coyoteTime;

            availableJumps = maxJumps;
            hasJumped = false;

            if (player.velocity.y < 0.25f){
                player.velocity = new Vector2(player.velocity.x, 0);
            }

        } else {
            coyoteTimeCounter -= Time.deltaTime;
        }
        //If you are not on the ground and have not jumped then you have walked off a ledge meaning you will have one less jump available.
        if ((coyoteTimeCounter < 0f) && isGrounded() == false && hasJumped == false){
            availableJumps = maxJumps-1;
        }

        //This makes it so the player falls faster the longer they're in the air, up to a certain speed.
        if (player.velocity.y < 0 && !isGrounded()){
            player.gravityScale = player.gravityScale * 1.005f;
        } else {
            player.gravityScale = 1;
        }
        player.velocity = new Vector2(player.velocity.x, Mathf.Max(player.velocity.y, -maxFallSpeed));

        //If you press jump it will set the jump buffer counter to the jump buffer time. It will then immeditally start counting down.
        if (Input.GetButtonDown("Jump")){
            jumpBufferCounter = jumpBufferTime;
        } else {
            jumpBufferCounter -= Time.deltaTime;
        }

        //If jumpBufferCounter is greater than 0, which it will be the frame you press space, and you have available jumps you then jump.
        if ((jumpBufferCounter > 0f) && availableJumps > 0){
            jumpBufferCounter = 0f;
            
            player.velocity = new Vector2(player.velocity.x, jumpForce);
            availableJumps--;
            hasJumped = true;

            coyoteTimeCounter = 0f;
        } 




        //HORIZONTAL MOVEMENT
        //Gets input from the arrow keys. -1 is left 1 is right
        direction = Input.GetAxisRaw("Horizontal");

        //If an arrow key is pressed it will add velocity to the player in whatever direction you have pressed.
        if (direction != 0f){
            transform.localScale = new Vector3(direction * scale.x, scale.y, scale.z);
            player.velocity = new Vector2(direction * walkSpeed, player.velocity.y);
        } else {
            player.velocity = new Vector2(0, player.velocity.y);
        }

        //If you press the attack key, it will resize a little hitbox to be big, then thens shrink it after a certain time.
        //Actual hits are detected in the HitDetection script of the hitbox object.
        if (Input.GetKeyDown(KeyCode.X)){
            Debug.Log("Attack!");
            
            //Once you press the attack key it resizes the hitbox so it can actually hit stuff.
            hitboxCollider.size = new Vector2(2f, 1.5f);
            hitboxCollider.offset = new Vector2(1.3f, 0);

            hitCounter = hitTime;
        } else {
            hitCounter -= Time.deltaTime;
        }

        if (hitCounter < 0){
            hitboxCollider.size = new Vector2(0, 0);
            hitboxCollider.offset = new Vector2(0, 0);
        }

        


        //Animation
        if (this.animator !=null){
            if (direction !=0 && isGrounded() && (Mathf.Abs(player.velocity.x) > 0.25f)){
                this.animator.speed = walkSpeed / 2.0f;
                this.animator.SetTrigger("Walking");
            }

            if (direction == 0 && isGrounded() && (Mathf.Abs(player.velocity.y)<0.25f)){
                this.animator.speed=1f;
                this.animator.SetTrigger("Idle");
            }

            if ((player.velocity.y > 0) && !isGrounded()){
                this.animator.speed=1f;
                this.animator.SetTrigger("Jumping");
            }

            if ((player.velocity.y < 0) && !isGrounded()){
                this.animator.speed=1f;
                this.animator.SetTrigger("Falling");
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D other) {
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
    }


    private bool isGrounded(){
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }
}
