using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
This script kind of needs to be in EVERY single room. It's basically how we can keep certain data across scenes. Other data like flags, items, hud, etc, can be put in here aswell.
This doesn't have to be the exact method we use, I mainly just needed a way to retain data across scenes. If there's a better option then thats fine. 
-James
*/
public class PlayerInfo : MonoBehaviour
{
    public static PlayerInfo pInfo;
    public int maxHealth = 5;
    public int currentHealth = 5;

    [HideInInspector]
    public Vector2 playerPos;


    [Header("Flags")]
    public int allowedJumps = 1;
    public bool hasDoubleJump;
    public bool hasWallJump;
    public bool hasMelee;
    public bool hasMeleeUpgrade;
    public bool hasWallBreak;
    public bool hasDash;
    public bool hasWarp;
    public int attackDamage;
    public float attackForce;


    public Hashtable oneWayDoors;

    [Header("Rooms")]
    public string lastRoom = "Room_Start";
    public string currentRoom;

    private void Awake(){
        
        
        if (pInfo !=null){
            Destroy(gameObject);
            return;
        }
        pInfo = this;
        DontDestroyOnLoad(gameObject);
        oneWayDoors = new Hashtable();
        oneWayDoors.Add("test", false);
        lastRoom = "Room_Start";

    }

    public void setLastRoom(string room){
        lastRoom = room;
    }

    public int getAllowedJumps(){
        return allowedJumps;
    }
}
