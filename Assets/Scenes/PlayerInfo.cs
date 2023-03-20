using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Class should pretty much always run. This doesn't have to stay, I just needed a way to keep the access lastRoom data. Should be attached to RoomManager.
If we scrap this and store the lastRoom info somewhere else then we need to edit the movement script and LoadManager.
*/
public class PlayerInfo : MonoBehaviour
{
    public static PlayerInfo pInfo;

    public string lastRoom = "";
    private int health;


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
