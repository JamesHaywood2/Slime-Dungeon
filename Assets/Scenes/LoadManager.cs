using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
Okay so when LoadManager starts, and it should be in every room so it should always start, it goes and finds a object called RoomManager.
The RoomManager contains the name of the last room/scene the player was in. Specifically the name/room the loadzone was in that brought the player to the new zone.
It will take that name and see if there are any load points (empty game objects named load_from_ROOMNAME) for that specific room.
If there are load points then it will will load them at that specific point. If there aren't then it will spawn them at the default load point.
The default is a child of LoadManager so it should also exist in every room.
*/
public class LoadManager : MonoBehaviour
{

    public GameObject player;
    RoomManager roomManager;


    // Start is called before the first frame update
    void Start()
    {
        GameObject RM = GameObject.Find("RoomManager");
        roomManager = RM.GetComponent<RoomManager>();
        
        GameObject loadSpot = GameObject.Find("load_from_" + roomManager.lastRoom);

       if (loadSpot != null){
            player.transform.position = new Vector3(loadSpot.transform.position.x, loadSpot.transform.position.y, transform.position.z);
       } else {
            loadSpot = GameObject.Find("load_from_Default");
            player.transform.position = new Vector3(loadSpot.transform.position.x, loadSpot.transform.position.y, transform.position.z);
       }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
