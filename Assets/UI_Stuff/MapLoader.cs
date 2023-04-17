using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


//I am past the point of caring lmao.


//It works. Here's an alternative implementation that would work better but that I'm too tired to do rn.
//Put all the room names into an array.
//Just make a VisualElement for ONLY the current room.
//At start iterate through the array and make a visual element, then immeditally set it to invisible.
//Then, when you enter a room it's just a matter of

public class MapLoader : MonoBehaviour
{

    public static MapLoader S;
    VisualElement root;

    //List of all room names.
    List<string> roomList = new List<string>();

    public Sprite playerIcon;




    private void Start() {
        S = this;
        root = GetComponent<UIDocument>().rootVisualElement;


        //Add all the room names to the list.
        roomList.Add("Room_MeleeUpgrade");
        roomList.Add("Room_DoubleJump");
        roomList.Add("Room_Dash");
        roomList.Add("Room_WallJump");
        roomList.Add("Room_WallBreak");
        roomList.Add("Room_Warp");
        roomList.Add("Room_Melee");
        roomList.Add("Room_Cannon");
        roomList.Add("Room_Start");
        roomList.Add("Room_1");
        roomList.Add("Room_2");
        roomList.Add("Room_3");
        roomList.Add("Room_4");
        roomList.Add("Room_5");
        roomList.Add("Room_6");
        roomList.Add("Room_7");
        roomList.Add("Room_8");
        roomList.Add("Room_9");
        roomList.Add("Room_10");
        roomList.Add("Room_11");
        roomList.Add("Room_12");
        roomList.Add("Room_13");
        roomList.Add("Room_14");
        roomList.Add("Room_15");
        roomList.Add("Room_16");
        roomList.Add("Room_17");
        roomList.Add("Room_18");
        roomList.Add("Room_19");
        roomList.Add("Room_20");
        roomList.Add("Room_21");
        roomList.Add("Room_Hard_1");
        roomList.Add("Room_Hard_2");
        roomList.Add("Room_Elevator_1");
        roomList.Add("Room_Elevator_2");
        roomList.Add("Room_Elevator_3");
        roomList.Add("Room_Elevator_4");
        roomList.Add("Room_Warp_1_1");
        roomList.Add("Room_Warp_1_2");
        roomList.Add("Room_Warp_2_1");
        roomList.Add("Room_Warp_2_2");
        roomList.Add("Room_Warp_3_1");
        roomList.Add("Room_Warp_3_2");
        roomList.Add("Room_Warp_4_1");
        roomList.Add("Room_Warp_4_2");
        

        //Go through roomList and make a visual element for each room. Initialize them to be invisible.
        foreach (string room in roomList) {
            root.Q<VisualElement>(room).style.display = DisplayStyle.None;
        }
        //Execpt start. That's always visible.
        root.Q<VisualElement>("Room_Start").style.display = DisplayStyle.Flex;
    }


    public void updateMap(){
        //Take the current room from PlayerInfo and set the corresponding room to be displayed.
        string cR = PlayerInfo.pInfo.currentRoom;
        
        if (root.Q<VisualElement>(cR) != null){
            VisualElement room = root.Q<VisualElement>(cR);
            room.style.display = DisplayStyle.Flex;
        
            //Set the room background image to playerIcon.
            room.style.backgroundImage = new StyleBackground(playerIcon);

            //set the last room style background image to blank.
            string lR = PlayerInfo.pInfo.lastRoom;
            VisualElement lastRoom = root.Q<VisualElement>(lR);
            lastRoom.style.backgroundImage = new StyleBackground();
        }


        
    }

    //Set every room to visible
    public void unlockMap(){
        foreach (string room in roomList) {
            root.Q<VisualElement>(room).style.display = DisplayStyle.Flex;
        }
    }

    public bool RoomTest(string roomName){
        //If the player has been in a room, then the room will be visible (Flex).
        if (root.Q<VisualElement>(roomName).style.display == DisplayStyle.Flex) {
            return true;
        } else {
            return false;
        }
    }

}
