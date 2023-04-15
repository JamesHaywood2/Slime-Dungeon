using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStairs : MonoBehaviour
{


    private GameObject enemyObject;
    private Enemy_Controller2 enemy;
    private bool bossDead;

    public bool stairsVisible;


    private void Awake() {
        enemyObject = GameObject.Find("Boss");
        enemy= enemyObject.GetComponent<Enemy_Controller2>();
        bossDead = enemy.isDead;
    }

    private void Update() {
        //If the boss is dead then make the stairs appear. Otherwise stairs are hidden.
        if (enemy.isDead){
            stairsVisible = true;
        }

        //Make the stairs tilemap and collider visible or invisible depending on the stairsVisible bool.
        if (stairsVisible){
            GetComponent<Renderer>().enabled = true;
            GetComponent<Collider2D>().enabled = true;
        } else {
            GetComponent<Renderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
        }

    }
}
