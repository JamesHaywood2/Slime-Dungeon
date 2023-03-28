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
        } 
    }

    private void OnTriggerExit2D(Collider2D other) {

    }
}
