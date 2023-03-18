using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Class should pretty much always run. This doesn't have to stay, I just needed a way to keep the access lastRoom data. Should be attached to RoomManager.
If we scrap this and store the lastRoom info somewhere else then we need to edit the movement script and LoadManager.
*/
public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    public string lastRoom = "";

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Awake(){
        
        if (Instance !=null){
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

    }

    public void setLastRoom(string room){
        lastRoom = room;
    }
}
