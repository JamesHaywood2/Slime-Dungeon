using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetection : MonoBehaviour
{
    public GameObject hitbox;
    private BoxCollider2D hitboxCollider;

    private GameObject objectHit;
    private void Start() {
        hitboxCollider = hitbox.GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "BreakableWall" && PlayerInfo.pInfo.hasWallBreak){
            Debug.Log(other.gameObject.name + " Has been destroyed!");
            Destroy(other.gameObject);
        }
    }
}
