using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Controller : MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        scale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        this.animator = GetComponent<Animator>();
        hitboxCollider = hitbox.GetComponent<BoxCollider2D>();

        enemy = GetComponent<Enemy>();
        enemy.returnSpot = RB.transform.position;


        chillCounter = chillTime;
    }

    // Update is called once per frame
    private void Update()
    {
        //If aggro is false then the player has not entered it's FOV and he's just patrolling/chilling in a set area.
        //If it's true then the skeleton will actively try to chase the enemy and attempt to fight them.
        if (enemy.isAggro == false && enemy.isReturning == false){

            if (enemy.isPatrolling == false){

                if (enemy.isWalking == true){
                    animator.speed = enemy.moveSpeed/1f;
                    animator.SetTrigger("Walking");
                    RB.velocity = new Vector2(direction * enemy.moveSpeed, RB.velocity.y);
                    transform.localScale = new Vector3(direction * scale.x, scale.y, scale.z);
                } else {
                    RB.velocity = new Vector2(0, RB.velocity.y);
                    animator.speed = enemy.moveSpeed/1f;
                    animator.SetTrigger("Idle");
                }
            } else {
                animator.SetTrigger("Walking");
                RB.velocity = new Vector2(direction * enemy.moveSpeed, RB.velocity.y);
                transform.localScale = new Vector3(direction * scale.x, scale.y, scale.z);
            }

        } else if (enemy.isAggro == true && enemy.isReturning == false){
            //If the enemy is aggroed it will go towards the player.
            //The only way to get out of aggro is to leave the enemy's FOV.

            //If the enemies x position is lower than the players x position they are too the left. else right.
            if (RB.position.x < PlayerInfo.pInfo.playerPos.x){
                direction = 1;
            } else {
                direction = -1;
            }


            if (attackCounter < 0f){
                
                if (Mathf.Abs(PlayerInfo.pInfo.playerPos.x - RB.position.x) <= enemy.effectiveRange.x &&
                Mathf.Abs(PlayerInfo.pInfo.playerPos.y - RB.position.y) <= enemy.effectiveRange.y)
                {
                    //If the player is in effective range, and is able to attack, then attack.
                    animator.speed = 1f/(hitTime*5f);
                    animator.SetTrigger("Attack");
                    attackCounter = enemy.attackSpeed;
                    RB.velocity = new Vector2(0, RB.velocity.y);

                    hitboxCollider.offset = enemy.hitBoxOffset;
                    hitboxCollider.size = enemy.hitBoxSize;

                    //How long the hitbox stays around.
                    hitCounter = hitTime;
                } else if (hitCounter > 0) {
                    //Not within effective range, and hitbox is gone, then move towards player.
                    animator.speed = 1f;
                    animator.SetTrigger("Walking");
                    RB.velocity = new Vector2(direction * enemy.moveSpeed, RB.velocity.y);
                    transform.localScale = new Vector3(direction * scale.x, scale.y, scale.z);
                } else {
                    //If you are not within the effective range, and hitbox is not gone, then do nothing.
                }


            } else {
                //If enemy can't attack just subtract time till they can.
                attackCounter -= Time.deltaTime;

                if (hitCounter < 0){
                    //Hitbox is gone
                    attackCounter -= Time.deltaTime;
                    hitboxCollider.offset = new Vector2(0f,0f);
                    hitboxCollider.size = new Vector2(0f,0f);
                } else {

                    hitCounter -= Time.deltaTime;
                    RB.velocity = new Vector2(0, RB.velocity.y);

                    RB.velocity = new Vector2(0, RB.velocity.y);
                    animator.speed = 1f;
                    animator.SetTrigger("Walking");
                    RB.velocity = new Vector2(direction * enemy.moveSpeed, RB.velocity.y);
                    transform.localScale = new Vector3(direction * scale.x, scale.y, scale.z);
                }
            }


        } else if (enemy.isReturning == true){

            if (Mathf.Abs(RB.position.x - enemy.returnSpot.x) < 0.25){
                enemy.isReturning = false;
            }

            if (RB.position.x < enemy.returnSpot.x){
                direction = 1;
            } else {
                direction = -1;
            }

            animator.speed = 1f;
            animator.SetTrigger("Walking");
            RB.velocity = new Vector2(direction * enemy.moveSpeed, RB.velocity.y);
            transform.localScale = new Vector3(direction * scale.x, scale.y, scale.z);


        }


    }

    private void FixedUpdate() {
        //If the enemy is not patrolling, does not have aggro, and is not returning to their default location
        //then generate a random number to determine if they should be walking or idling.
        if (enemy.isPatrolling == false && enemy.isAggro == false && enemy.isReturning==false){
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
                chillCounter -= Time.fixedDeltaTime;
            }

        }


    }

    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Skeleton collided with: " + other.name);
        
        if (enemy.isAggro == false && enemy.isReturning == false && other.tag == "EnemyBumper" ){
            if (RB.position.x - other.transform.position.x < 0){
                direction = -1;
            } else {
                direction = 1;
            }
        } else if (other.tag == "EnemyBumperAbs"){
            if (RB.position.x - other.transform.position.x < 0){
                direction = -1;
            } else {
                direction = 1;
            }
            enemy.isAggro = false;
            enemy.isReturning = true;
        } else if (other.tag == "JumpPad"){
            
            RB.velocity = new Vector2(RB.velocity.x, other.GetComponent<JumpPad>().jumpPower);
        }
    }


}

