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
    public bool hasWallJump;
    public bool hasMelee;
    public bool hasWallBreak;
    public bool hasDash;
    public bool hasWarp;
    public int attackDamage;
    public float attackForce;


    [Header("Rooms")]
    public string lastRoom;
    public string currentRoom;
    public bool room_Start;
    public bool room_Melee;
    public bool room_WallBreak;
    public bool room_WallJump;
    public bool room_DoubleJump;
    public bool room_Dash;
    public bool room_WarpUnlock;

    public bool room_1;
    public bool room_2;


    private void Awake(){
        
        if (pInfo !=null){
            Destroy(gameObject);
            return;
        }
        pInfo = this;
        DontDestroyOnLoad(gameObject);

    }

    public void setLastRoom(string room){
        lastRoom = room;
    }

    public int getAllowedJumps(){
        return allowedJumps;
    }
}
