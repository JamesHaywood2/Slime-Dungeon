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


    [Header("Ground checking variables")]
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundLayer;
    private bool hasJumped;

    [Header("Respawn/Checkpoints")]
    private Transform checkPoint;
    private Transform respawnPoint;


    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Rigidbody2D>();
        scale = new Vector3(player.transform.localScale.x, player.transform.localScale.y, player.transform.localScale.z);

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
            availableJumps = maxJumps;
            hasJumped = false;
        } 
        //If you are not on the ground and have not jumped then you have walked off a ledge meaning you will have one less jump available.
        if (isGrounded() == false && hasJumped == false){
            availableJumps = maxJumps-1;
        }

        //This makes it so the player falls faster the longer they're in the air, up to a certain speed.
        if (player.velocity.y < 0 && !isGrounded()){
            player.gravityScale = player.gravityScale * 1.005f;
        } else {
            player.gravityScale = 1;
        }
        player.velocity = new Vector2(player.velocity.x, Mathf.Max(player.velocity.y, -maxFallSpeed));

        //If you press the button Jump is assigned to in project input settings and you have more than 0 jumps you will jump.
        //After jumping it will set hasJumped to true and decrease your available jumps.
        if (Input.GetButtonDown("Jump") && availableJumps > 0){
            
            player.velocity = new Vector2(player.velocity.x, jumpForce);
            availableJumps--;
            hasJumped = true;
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


        //Animation
        if (this.animator !=null){
            if (direction !=0 && isGrounded() && (Mathf.Abs(player.velocity.x) > 0.25)){
                this.animator.SetTrigger("Walking");
            }

            if (direction == 0 && isGrounded() && (Mathf.Abs(player.velocity.y)<0.25)){
                this.animator.SetTrigger("Idle");
            }

            if ((player.velocity.y > 0) && !isGrounded()){

                this.animator.SetTrigger("Jumping");
            }

            if ((player.velocity.y < 0) && !isGrounded()){

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
