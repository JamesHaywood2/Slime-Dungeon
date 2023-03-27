using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Controller2 : MonoBehaviour
{
    Rigidbody2D RB;
    private Vector3 scale;
    Animator animator;
    private Enemy enemy;
    public Collider2D FOV;
    public Collider2D aggroRange;
    public GameObject hitbox;
    private BoxCollider2D hitboxCollider;
    public int direction = 1;


    private float chillTime = 1f;
    private float chillCounter;

    public float attackCounter;
    public float hitCounter;
    public float hitTime = 0.4f;

    private bool isMoving;


    // Start is called before the first frame update
    void Start()
    {
        //Just gets components and sets the returnSpot.
        RB = GetComponent<Rigidbody2D>();
        scale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        this.animator = GetComponent<Animator>();
        hitboxCollider = hitbox.GetComponent<BoxCollider2D>();

        enemy = GetComponent<Enemy>();
        enemy.returnSpot = RB.transform.position;
        enemy.hasReturned = true;


        chillCounter = chillTime;
    }

    // Update is called once per frame
    void Update()
    {
        //If aggro is false, and enemy is not returning to his designated location,
        //then he exhibits normal non aggressive behavior within his designated bounds. Bounds are defined by the GameObject enemy bumper tagged with the EnemyBumper tag.
        //These bumpers just swap the the direction the enemy is facing and thus the direction they're moving.
        if (enemy.isAggro == false && enemy.isReturning == false){
            //The enemies have 3 basic non-aggressive states.
            //The first state is called patrolling. This means they will just walk back and forth until they hit a bumper and continue patrolling an area.
            //The second state is called chilling. This means they are swapping between idle and walking animations and walking around their designated area.
            //The third stage is when they are not patrolling or idling. They just stand in place and idle until aggro is drawn.
            if (enemy.isPatrolling == true){
                enemy.isWalking = true;
            } else if (enemy.isChilling == true){
                //If enemy is not patrolling then they must be just chilling. Generate a random number every time chillTime reaches zero.
                if (chillCounter < 0f){
                    chillCounter = chillTime;
                    //If the randomly generated number triggers, then it swaps between walking and idling.
                    float rand = Random.Range(0.00f, 1.00f);
                    if (rand <= 0.45f){
                        if (Random.Range(0.00f, 1.00f) < 0.5f){
                            direction *= -1;
                        }
                        enemy.isWalking = !enemy.isWalking;
                    }

                } else {
                    chillCounter -= Time.deltaTime;
                }
            } else {
                enemy.isWalking=false;
            }


        } else if (enemy.isAggro == true && enemy.isReturning == false){
           //As soon as the player enters an enemies aggro range, indicated by a child GameObject containing a boxcollider, it will set is Aggro to true.
           //It does this in PlayerController2.cs.

            //This gets the direction in which the enemy needs to walk in order to get to the player.
            //NOTE: There is a technical edge case that I don't want to deal with: 
            //|------------------------|
            //|             P          |
            //|     -------------------|
            //|             E          |
            //| _______________________|
            //If the enemy is directly below (or above) the player like in above they won't actually go to the player, just stand below/above them.
            //This can be solved with actual enemy pathing, but I don't want to deal with that. -James
            //If the enemies x position is lower than the players x position they are too the left. else right.
            if (RB.position.x < PlayerInfo.pInfo.playerPos.x){
                direction = 1;
            } else {
                direction = -1;
            }

            //So as soon as the enemy is aggroed it should leave it's spot to try and get to the player.
            enemy.hasReturned = false;

            //attackCounter <0f means the enemy is able to attack (attack is not on cooldown).
            if (attackCounter < 0f){
                //This checks if the player is within an effective range of the enemies attack
                if (Mathf.Abs(PlayerInfo.pInfo.playerPos.x - RB.position.x) <= enemy.effectiveRange.x &&
                Mathf.Abs(PlayerInfo.pInfo.playerPos.y - RB.position.y) <= enemy.effectiveRange.y)
                {
                    //If the player is in effective range, and is able to attack, then attack.
                    animator.speed = 1f/(hitTime*5f);
                    animator.SetTrigger("Attack");
                    attackCounter = enemy.attackSpeed;
                    enemy.isWalking=false;

                    hitboxCollider.offset = enemy.hitBoxOffset;
                    hitboxCollider.size = enemy.hitBoxSize;

                    //How long the hitbox stays around.
                    hitCounter = hitTime;
                } else if (hitCounter <0f){
                    //Not within effective range, and hitbox is gone (attack finished), then move towards player.
                    enemy.isWalking=true;
                } else {
                    enemy.isWalking = false;
                }

            } else {
                //If attack is on cooldown (can't attack)
                if (hitCounter < 0){
                    //and the hitbox is hitbox is gone (attack finished), then set the hitbox size and everything to 0 and subtract time from attack cooldown.
                    attackCounter -= Time.deltaTime;
                    hitboxCollider.offset = new Vector2(0f,0f);
                    hitboxCollider.size = new Vector2(0f,0f);
                } else {
                    //and the hitbox is not gone(attack in progress), then do not move the player.
                    hitCounter -= Time.deltaTime;
                    enemy.isWalking = false;
                }
            }

        } else if (enemy.isReturning == true){
            //If the enemy is Not aggro, and is trying to return to it's original location, then

            //Check if the enemy has made it back to it's original location (x value only). If it then it goes back to it's default.
            if (Mathf.Abs(RB.position.x - enemy.returnSpot.x) < 0.25){
                enemy.isReturning = false;
                enemy.isWalking = false;
                enemy.hasReturned = true;
            }
            //Gets direction of the spot.
            if (RB.position.x < enemy.returnSpot.x){
                direction = 1;
            } else {
                direction = -1;
            }
            //Sets the enemy to start walking to the spot.
            enemy.isWalking = true;

        }

        if (enemy.isWalking == true){
            animator.speed = enemy.moveSpeed/1f;
            animator.SetTrigger("Walking");
            transform.localScale = new Vector3(direction * scale.x, scale.y, scale.z);
        } else {
            animator.speed = 1f;
            animator.SetTrigger("Idle");
        }

    }

    private void FixedUpdate() {
        if (enemy.isWalking){
            RB.velocity = new Vector2(direction * enemy.moveSpeed, RB.velocity.y);
        } else {
            RB.velocity = new Vector2(0, RB.velocity.y);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other) {    
        if (enemy.isAggro == false && enemy.isReturning == false && other.tag == "EnemyBumper" && enemy.hasReturned == true){
            // if (RB.position.x - other.transform.position.x < .5f){
            //     direction = -1;
            // } else {
            //     direction = 1;
            // }
            direction *= -1;
            RB.transform.position += new Vector3(-direction * RB.GetComponent<CapsuleCollider2D>().offset.x + (direction * (RB.GetComponent<CapsuleCollider2D>().size.x)/2), 0f, 0f);
        } else if (other.tag == "EnemyBumperAbs"){
            direction *= -1;
            RB.transform.position += new Vector3(-direction * RB.GetComponent<CapsuleCollider2D>().offset.x + (direction * (RB.GetComponent<CapsuleCollider2D>().size.x)/2), 0f, 0f);
            
            enemy.isAggro = false;
            //enemy.isReturning = true;
        } else if (other.tag == "JumpPad"){
            RB.velocity = new Vector2(RB.velocity.x, other.GetComponent<JumpPad>().jumpPower);
        }
        //getting hit/taking damage?
    }
}
