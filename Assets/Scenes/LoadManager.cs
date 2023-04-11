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
     }
}
