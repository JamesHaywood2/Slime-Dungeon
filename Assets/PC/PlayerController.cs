using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    LoadZoneInfo loadZone;

    Rigidbody2D rigid2D;
    Animator animator;
    GameObject WallCheck;
    private float jumpForce = 435.0f;
    private float walkSpeed = 6.0f;

    private int grounded;
    private int jumps;
    private int jumping;
    private int falling;

    // Start is called before the first frame update
    void Start()
    {
        this.rigid2D = GetComponent<Rigidbody2D>();
        this.animator = GetComponent<Animator>();
        jumps = PlayerInfo.pInfo.getAllowedJumps();
    }

    // Update is called once per frame
    void Update()
    {


        float speedx = Mathf.Abs(this.rigid2D.velocity.x);
        float speedy = Mathf.Abs(this.rigid2D.velocity.y);
        
        // Jump
        /*
        Double jumping doesnt work that well right now.
        */
        if (Input.GetKeyDown(KeyCode.Space) && jumps>0)
        {
            this.rigid2D.AddForce(transform.up * this.jumpForce);
            jumps--;
            jumping = 1;
            falling = 0;
        }


        if (this.rigid2D.velocity.y <0 && grounded==0){
            this.animator.SetTrigger("Falling");
            jumping = 0;
            falling = 1;
        }

        float unitsThisFrame = walkSpeed * Time.deltaTime;
        int key=0;
        if (Input.GetKey(KeyCode.RightArrow)) key = 1;
        if (Input.GetKey(KeyCode.LeftArrow)) key = -1;


        Vector3 pos;
        pos.x = Input.GetAxis("Horizontal");
        pos.y=0.0f;
        pos.z=0.0f;

        transform.position += pos * unitsThisFrame;
        // reverse spring according to the direction
        if (key != 0)
        {
            transform.localScale = new Vector3(key*0.35f, 0.35f, 0.35f);
        } 

        if (this.animator !=null){
            if (key !=0 && grounded==1){
                this.animator.SetTrigger("Walking");
            }

            if (key ==0 && grounded==1){
                this.animator.SetTrigger("Idle");
            }

            if (jumping == 1){
                this.animator.SetTrigger("Jumping");
            }

            if (falling == 1){
                this.animator.SetTrigger("Falling");
            }

        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {

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

        } else if (other.tag == "Terrain"){
            grounded = 1;
            jumps=PlayerInfo.pInfo.getAllowedJumps();
            Debug.Log("You are grounded");
            jumping = 0;
            falling = 0;
        }
    }

    void OnTriggerExit2D(Collider2D other){
        if (other.tag == "Terrain"){
            grounded=0;
            jumps=PlayerInfo.pInfo.getAllowedJumps()-1;
            Debug.Log("Left the ground");
        }
    }

}
