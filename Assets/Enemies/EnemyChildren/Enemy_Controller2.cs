using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Controller2 : MonoBehaviour
{
    Rigidbody2D RB;
    private Vector3 scale;
    private Animator animator;
    private Enemy enemy;
    private BoxCollider2D hitboxCollider;
    private BoxCollider2D hurtboxCollider;
    private int direction = 1;

    public bool isDead;

    [SerializeField]private float chillTime = 1f;
    private float chillCounter;

    [Header("Attack")]
    private float attackCounter;
    [SerializeField]private float hitTime;
    private float hitCounter;
    private bool isHit;

    [Header("Ranges")]
    public float aggroRadius;
    public float fovRadius;
    public float attackRange;
    private float distance;

    


    // Start is called before the first frame update
    void Start()
    {
        //Just gets components and sets the returnSpot.
        RB = GetComponent<Rigidbody2D>();
        scale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        this.animator = GetComponent<Animator>();

        enemy = GetComponent<Enemy>();
        enemy.returnSpot = RB.transform.position;
        enemy.hasReturned = true;


        hitboxCollider = transform.Find("Hitbox").GetComponent<BoxCollider2D>();
        hitboxCollider.enabled = false;
        hurtboxCollider = transform.Find("Hurtbox").GetComponent<BoxCollider2D>();


        chillCounter = chillTime;
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(PlayerInfo.pInfo.playerPos, this.transform.position);
        inAggroRange();
        inFovRange();



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
            direction = directionOfPlayer();

            //So as soon as the enemy is aggroed it should leave it's spot to try and get to the player.
            enemy.hasReturned = false;

            //attackCounter <0f means the enemy is able to attack (attack is not on cooldown).
            if (attackCounter < 0f){
                hitboxOff();
                //This checks if the player is within an attack range of the enemies attack
                if (Mathf.Abs(PlayerInfo.pInfo.playerPos.x - RB.position.x) <= attackRange)
                {
                    //If the player is in the attack x range, then stop moving and just wait for the player to fall.
                    enemy.isWalking=false;
                    animator.SetBool("Walking",false);
                    animator.SetTrigger("Idle");

                    if (Mathf.Abs(PlayerInfo.pInfo.playerPos.y - RB.position.y) <= attackRange){
                        direction = directionOfPlayer();
                        animator.speed = 1f;
                        animator.SetTrigger("Attack");
                        attackCounter = enemy.attackSpeed;
                    }
                    
                    //hitboxOn() should be called as an event in the animation and hitboxOff() at the end of it.


                } else {
                    //If player is not in attack x range, then move till they are.
                    enemy.isWalking=true;
                    animator.SetBool("Walking",true);
                }

            } else if (hitboxCollider.enabled == true) {
                //If the hitbox is enabled then the attack is in progress.
                //If the hitbox is disabled then the attack either hasn't started.
                //We want the attack animation to continue and finish if it's active.
                //Debug.Log("Attack in progress");

            } else {
                //If the player can't attack, then it attack is on cooldown.
                //If attack is on cooldown (can't attack)
                attackCounter -= Time.deltaTime;
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

        if (hitCounter < 0f){
            //If hitCounter is less than 0 then the player shouldn't be playing the hit animation.
            //Or when hit counter reaches zero it turns isHit off.
            isHit = false;
            animator.SetBool("Hit", false);
        } else {
            hitCounter -= Time.deltaTime;
        }

        if (isHit){
        }
        //If the enemy isWalking, play the walking animation and set Walking animation to true.
        else if (enemy.isWalking == true){
            animator.speed = enemy.moveSpeed/1f;
            animator.SetBool("Walking",true);
            if (!isDead){
                transform.localScale = new Vector3(direction * scale.x, scale.y, scale.z);
            }
        } else {
            animator.speed = 1f;
            animator.SetTrigger("Idle");
            animator.SetBool("Walking",false);
        }

        if (isDead){
            enemy.isWalking = false;
            enemy.moveSpeed = 0f;
            hitboxCollider.enabled = false;
            hurtboxCollider.enabled = false;
        }

        if (enemy.health <= 0){
                animator.SetTrigger("Killed");
                animator.SetBool("Dead", true);
                isDead = true;
                hitboxCollider.enabled = false;
                

                if (enemy.type == Enemy.EnemyType.Skeleton 
                || enemy.type == Enemy.EnemyType.Goblin
                || enemy.type == Enemy.EnemyType.Mushroom){
                    //If the enemy is a Skeleton then just resize the hitbox to 0.
                    //The ending animation for these is just a corpse so it can actually stay and not look stupid.
                    aggroRadius=0;
                    fovRadius=0;
                    attackRange=0;
                    //Add a hitbox around the enemy that damages player if they touch. Set it to zero here.
                    //this.GetComponent<CapsuleCollider2D>().size = new Vector2(0.5f, 0.5f);
                    //this.GetComponent<CapsuleCollider2D>().offset = new Vector2(0f, -((this.GetComponent<SpriteRenderer>().bounds.size.y/2) + 0.5f));
                    this.gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
                } else {
                    Destroy(this);
                }
            }

    }

    private void FixedUpdate() {
        //If the enemy was hit, and hitCounter (iframes or something?) then they just go in the direction they were hit.
        //Note, may not actually hit them opposite of the player. :) - james.
        if (isHit == true){
            //RB.velocity = new Vector2(-direction * 1f, .25f);
        } else if (enemy.isWalking){
            RB.velocity = new Vector2(direction * enemy.moveSpeed, RB.velocity.y);
        } else {
            RB.velocity = new Vector2(0, RB.velocity.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {    
        //Debug.Log("Other: " + other.tag);
        //Debug.Log("Parent: " + other.transform.parent.tag);
        if (enemy.isAggro == false && enemy.isReturning == false && other.tag == "EnemyBumper" && enemy.hasReturned == true){
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
        else if (other.tag == "HitBox" && other.transform.parent.tag == "Player"){
            if (isHit == false){
                hitboxOff();
                enemy.health -= PlayerInfo.pInfo.attackDamage;
                hitCounter = hitTime;
                isHit = true;

                // how much the character should be knocked back is PlayerInfo.pInfo.attackForce
                // calculate force vector
                var force = transform.position - other.transform.position;
                // normalize force vector to get direction only and trim magnitude
                force.Normalize();
                RB.AddForce(force * PlayerInfo.pInfo.attackForce * 10);

                
                //If the player took a hit, play the animation.
                animator.speed = 1f;
                animator.SetBool("Hit", true);
                animator.SetTrigger("damaged");
                hitboxCollider.enabled = false;

            } else {
                Debug.Log("Enemy was already hit");
            }
        }
    }

    private void hitboxOn(){
        hitboxCollider.enabled = true;
    }

    private void hitboxOff(){
        hitboxCollider.enabled = false;
    }

    private void inAggroRange(){
        if (distance <= aggroRadius){
            enemy.isAggro = true;
            enemy.isReturning = false;
        }
    }

    private void inFovRange(){
        if ( !(distance <= fovRadius) ){
            //Player has left the FOV range.
            if (enemy.isAggro){
                enemy.isAggro = false;
                enemy.isReturning = true;
            }
        }
    }
    
    private int directionOfPlayer(){
        int d = 0;
        if (RB.position.x < PlayerInfo.pInfo.playerPos.x){
                d = 1;
        } else {
                d = -1;
        }
        return d;
    }

     #region EDITOR METHODS
    void OnDrawGizmosSelected()
    {
        //Draw the aggroRange
		Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRadius);
        //draw the FOV range.
		Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, fovRadius);
        //draw the attackRange
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange);
	}
    #endregion
}
