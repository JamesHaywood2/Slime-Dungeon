using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
Load manager just looks at the object PlayerInfo, which has the PlayerInfo.cs script. This contains general player information.
It takes the lastRoom data and will then try and find the correct loadspot on the level. If that loadspot exists in the level then it will teleport the player there
upon loading into the room. If it does not exist then it just teleports the player to the default position.
*/
public class LoadManager : MonoBehaviour
{

    private GameObject player;

    // Start is called before the first frame update
    void Start(){
          GameObject loadSpot = GameObject.Find("load_from_" + PlayerInfo.pInfo.lastRoom);
          player = GameObject.Find("Player");

          if (loadSpot != null){
               player.transform.position = new Vector3(loadSpot.transform.position.x, loadSpot.transform.position.y, transform.position.z);
          } else {
               loadSpot = GameObject.Find("load_from_Default");
               player.transform.position = new Vector3(loadSpot.transform.position.x, loadSpot.transform.position.y, transform.position.z);
          }

          //Unload those oneway doors that have been used.
          foreach (DictionaryEntry entry in PlayerInfo.pInfo.oneWayDoors){
               if (entry.Value.Equals(true)){
                    //The key is in the format of name of the current room from PlayerInfo and name of the door. So we need to split it up.
                    string doorName = entry.Key.ToString().Split('*')[1];
                    Debug.Log(doorName);
                    //Now we have the name of the current room and the name of the door. We can use this to find the door in the scene.
                    GameObject door = GameObject.Find(doorName);
                    Destroy(door);
               }
          }

          //Find the warphole gameobject. If the player doesn't have the warp ability then destroy it.
          if (!PlayerInfo.pInfo.hasWarp){
               GameObject warpHole = GameObject.Find("WarpHole");
               Destroy(warpHole);
          }

          if (PlayerInfo.pInfo.currentHealth < PlayerInfo.pInfo.maxHealth){
                    PlayerInfo.pInfo.currentHealth += 1;
          }

     }

     private void LateUpdate() {
          MapLoader.S.updateMap();
     }
}
