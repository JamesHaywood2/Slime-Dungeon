using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

     public enum EnemyType{
        Skeleton,
        Cultist,
        Worm,
        Goblin,
        Mushroom,
    };

    
    public EnemyType type;
    public int health;
    public float moveSpeed;

    [Header("Hitbox Info")]
    public Vector2 hitBoxSize;
    public Vector2 hitBoxOffset;
    public Vector2 effectiveRange;
    public float attackSpeed;

    [Header("State flags")]
    //If walking then they are walking in direction. If not they are idling.
    public bool isWalking;
    //isChilling is like the opposite of isPatrolling.
    public bool isChilling;
    //If they are patrolling then they are walking back and forth between a set area and not chilling.
    //If they are chilling then they are just swapping between idle/walk animations in a set area.
    public bool isPatrolling;
    //true if they are aggroed on the player.
    //If they are aggroed then isPatrolling becomes irrelevant.
    //The enemy will chase the player as long as they are in their FOV and haven't touched an absolute bumper.
    //NOTE: A normal bumper is ignored if enemy is aggroed. Absolute bumpers are not ignored. Absolute bumpers are to stop enemies from just walking off a ledge.
    public bool isAggro;
    //True if they lost their agro.
    public bool isReturning;
    //Spot that the enemy attempts returns to after losing agro.
    public Vector2 returnSpot;
    public bool hasReturned;
}