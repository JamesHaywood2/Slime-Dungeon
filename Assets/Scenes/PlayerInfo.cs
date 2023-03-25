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
    public string lastRoom = "";
    public int maxHealth = 5;
    public int currentHealth = 5;


    [Header("Flags")]
    public int allowedJumps = 1;
    public bool hasWallJump;
    public bool hasMelee;
    public bool hasDash;
    public bool Warp;
    public int attackDamage;
    public bool hasWallBreak;




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
