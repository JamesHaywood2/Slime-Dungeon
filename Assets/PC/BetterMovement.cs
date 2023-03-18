using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BetterMovement : MonoBehaviour
{

    SceneLoader zoneLoader;

    Rigidbody2D rigid2D;
    Animator animator;
    float jumpForce = 420.0f;

    public float walkSpeed = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        this.rigid2D = GetComponent<Rigidbody2D>();
        this.animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {


        float speedx = Mathf.Abs(this.rigid2D.velocity.x);
        
        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && this.rigid2D.velocity.y == 0)
        {
            this.rigid2D.AddForce(transform.up * this.jumpForce);
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
            transform.localScale = new Vector3(key, 1, 1);
        } 

        if (this.animator !=null){
            if (key !=0){
                this.animator.SetTrigger("Walking");
            }

            if (key ==0){
                this.animator.SetTrigger("Idle");
            }
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "LoadZone")
        {
            
            Debug.Log("Entered Loadzone: " + other.name);
            GameObject LZ = GameObject.Find(other.name);
            zoneLoader = LZ.GetComponent<SceneLoader>();
            SceneManager.LoadScene(zoneLoader.Destination);

        }
    }
}
