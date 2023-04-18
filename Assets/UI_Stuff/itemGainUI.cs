using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class itemGainUI : MonoBehaviour
{
    public static itemGainUI S;
    public Button dashButton;
    public Button doubleJumpButton;
    public Button wallJumpButton;
    public Button meleeButton;
    public Button meleeUpgradeButton;
    public Button wallBreakButton;
    public Button warpButton;


    public enum itemType{
        dash,
        doubleJump,
        wallJump,
        melee,
        meleeUpgrade,
        wallBreak,
        warp
    }

    VisualElement root;

    private void Start() {
        S=this;
        root = GetComponent<UIDocument>().rootVisualElement;

        root.Q<VisualElement>("dashInfo").style.display = DisplayStyle.None;
        root.Q<VisualElement>("doubleJumpInfo").style.display = DisplayStyle.None;
        root.Q<VisualElement>("wallJumpInfo").style.display = DisplayStyle.None;
        root.Q<VisualElement>("meleeInfo").style.display = DisplayStyle.None;
        root.Q<VisualElement>("meleeUpgradeInfo").style.display = DisplayStyle.None;
        root.Q<VisualElement>("wallBreakInfo").style.display = DisplayStyle.None;
        root.Q<VisualElement>("warpInfo").style.display = DisplayStyle.None;


        dashButton = root.Q<Button>("dashButton");
        doubleJumpButton = root.Q<Button>("doubleJumpButton");
        wallJumpButton = root.Q<Button>("wallJumpButton");
        meleeButton = root.Q<Button>("meleeButton");
        meleeUpgradeButton = root.Q<Button>("meleeUpgradeButton");
        wallBreakButton = root.Q<Button>("wallBreakButton");
        warpButton = root.Q<Button>("warpButton");

        dashButton.clicked += DashButtonPressed;
        doubleJumpButton.clicked += DoubleJumpButtonPressed;
        wallJumpButton.clicked += WallJumpButtonPressed;
        meleeButton.clicked += MeleeButtonPressed;
        meleeUpgradeButton.clicked += MeleeUpgradeButtonPressed;
        wallBreakButton.clicked += WallBreakButtonPressed;
        warpButton.clicked += WarpButtonPressed;


        
    }


    public void DisplayInfo(itemType item){
        Time.timeScale = 0;
        switch(item){
            case itemType.dash:
                root.Q<VisualElement>("dashInfo").style.display = DisplayStyle.Flex;
                break;
            case itemType.doubleJump:
                root.Q<VisualElement>("doubleJumpInfo").style.display = DisplayStyle.Flex;
                break;
            case itemType.wallJump:
                root.Q<VisualElement>("wallJumpInfo").style.display = DisplayStyle.Flex;
                break;
            case itemType.melee:
                root.Q<VisualElement>("meleeInfo").style.display = DisplayStyle.Flex;
                break;
            case itemType.meleeUpgrade:
                root.Q<VisualElement>("meleeUpgradeInfo").style.display = DisplayStyle.Flex;
                break;
            case itemType.wallBreak:
                root.Q<VisualElement>("wallBreakInfo").style.display = DisplayStyle.Flex;
                break;
            case itemType.warp:
                root.Q<VisualElement>("warpInfo").style.display = DisplayStyle.Flex;
                break;
            default:
                break;
        }
        
    }

    void DashButtonPressed(){
        root.Q<VisualElement>("dashInfo").style.display = DisplayStyle.None;
        Time.timeScale = 1;
    }

    void DoubleJumpButtonPressed(){
        root.Q<VisualElement>("doubleJumpInfo").style.display = DisplayStyle.None;
        Time.timeScale = 1;
    }

    void WallJumpButtonPressed(){
        root.Q<VisualElement>("wallJumpInfo").style.display = DisplayStyle.None;
        Time.timeScale = 1;
    }

    void MeleeButtonPressed(){
        root.Q<VisualElement>("meleeInfo").style.display = DisplayStyle.None;
        Time.timeScale = 1;
    }

    void MeleeUpgradeButtonPressed(){
        root.Q<VisualElement>("meleeUpgradeInfo").style.display = DisplayStyle.None;
        Time.timeScale = 1;
    }

    void WallBreakButtonPressed(){
        root.Q<VisualElement>("wallBreakInfo").style.display = DisplayStyle.None;
        Time.timeScale = 1;
    }

    void WarpButtonPressed(){
        root.Q<VisualElement>("warpInfo").style.display = DisplayStyle.None;
        Time.timeScale = 1;
    }


    



}
