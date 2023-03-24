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
    public int allowedJumps = 1;
    private int health;


    //Flags
    private int hasDoubleJump;
    private int hasWallJump;
    private int hasMelee;




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
}
