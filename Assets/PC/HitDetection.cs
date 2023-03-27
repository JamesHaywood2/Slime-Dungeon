using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetection : MonoBehaviour
{
    public GameObject hitbox;
    private BoxCollider2D hitboxCollider;
    //Owner of the hitbox
    private GameObject owner;
    //damage the hitbox has done.
    private int damage;

    //Object that was hit.
    private GameObject objectHit;


    private void Start() {
        hitboxCollider = hitbox.GetComponent<BoxCollider2D>();
        owner = hitbox.transform.parent.gameObject;
    }

    private void OnTriggerEnter2D(Collider2D other) {

        objectHit = other.gameObject;
        if (objectHit.tag == "BreakableWall" && PlayerInfo.pInfo.hasWallBreak && owner.tag == "Player"){
            Debug.Log(other.gameObject.name + " Has been destroyed!");
            Destroy(other.gameObject);
        } else if (objectHit.tag == "Player" && owner.tag != "FOV"){
            //If the hitbox that is being hit is the Player, and it's not the FOV hitbox.
            PlayerInfo.pInfo.currentHealth = PlayerInfo.pInfo.currentHealth - damage;
        } else if (objectHit.tag == "Enemy"){
            //If the hitbox that is being hit is the enemy.
            Enemy enemy = owner.GetComponent<Enemy>();
            enemy.health = enemy.health - damage;
        } 
    }

    private void OnTriggerExit2D(Collider2D other) {

    }
}
