using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemCollection : MonoBehaviour
{
    private void Start() {
        //Get the tag of the item
        string itemTag = gameObject.tag;
        switch(itemTag){
            case "dashItem":
                if (PlayerInfo.pInfo.hasDash == true){
                    Destroy(gameObject);
                }
                break;
            case "doubleJumpItem":
                if (PlayerInfo.pInfo.hasDoubleJump == true){
                    Destroy(gameObject);
                }
                break;
            case "wallJumpItem":
                if (PlayerInfo.pInfo.hasWallJump == true){
                    Destroy(gameObject);
                }
                break;
            case "meleeItem":
                if (PlayerInfo.pInfo.hasMelee == true){
                    Destroy(gameObject);
                }
                break;
            case "meleeUpgradeItem":
                if (PlayerInfo.pInfo.hasMeleeUpgrade == true){
                    Destroy(gameObject);
                }
                break;
            case "wallBreakItem":
                if (PlayerInfo.pInfo.hasWallBreak == true){
                    Destroy(gameObject);
                }
                break;
            case "warpItem":
                if (PlayerInfo.pInfo.hasWarp == true){
                    Destroy(gameObject);
                }
                break;
            default:
                break;
        }
    }
}
