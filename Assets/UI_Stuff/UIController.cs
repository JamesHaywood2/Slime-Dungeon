using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public static UIController S;

    public enum MenuState
    {
        SettingsMenu,
        PauseMenu,
        devMenu,
        none
    };
    
    public MenuState activeMenu;

    //PAUSE MENU
    public bool isPaused = false;
    public Button settingsButton;
    public Button resumeButton;
    public Button menuButton;
    public Button secretButton;
    private int secretCount;
    public VisualElement pauseMenu;
    
    //item labels in pauseMenu
    public Label MeleeLabel;
    public Label MeleeUpgradeLabel;
    public Label DoubleJumpLabel;
    public Label DashLabel;
    public Label WallJumpLabel;
    public Label WallBreakLabel;
    public Label WarpLabel;
    

    //HPBAR
    public VisualElement hpBar;
    public Label hpLabel;
    private float hpBarCurrentWidth;

    //Dev Menu
    public VisualElement devMenu;
    public Button healButton;
    public Toggle wallJumpToggle;
    public Toggle wallBreakToggle;
    public Toggle dashToggle;
    public Toggle warpToggle;
    public Toggle meleeToggle;
    public TextField teleportDestination;
    public Button teleportButton;
    public Button unlockRoomsButton;

    VisualElement root;


    // Start is called before the first frame update
    void Start()
    {
        //Assigns the UI document the script is attached to to root.
        root = GetComponent<UIDocument>().rootVisualElement;

        activeMenu = MenuState.none;

        //PAUSE MENU VARIABLES
        //Assign variables for Pause Menu elements.
        pauseMenu = root.Q<VisualElement>("pauseMenu");
        //Button to open settings menu.
        settingsButton = root.Q<Button>("settingsButton");
        //Button to unpause the game. Can also unpause by pressing escape again.
        resumeButton = root.Q<Button>("resumeButton");
        //Button to send player to main menu.
        menuButton = root.Q<Button>("menuButton");
        //Button to send player to dev menu.
        secretButton = root.Q<Button>("secretButton");

        settingsButton.clicked += SettingsButtonPressed;
        resumeButton.clicked += ResumeButtonPressed;
        menuButton.clicked += MenuButtonPressed;
        secretButton.clicked += SecretButtonPressed;

        //Health Bar Variables
        hpLabel = root.Q<Label>("hpExact");
        hpBar = root.Q<VisualElement>("hpFill");

        //item labels
        MeleeLabel = root.Q<Label>("MeleeLabel");
        MeleeUpgradeLabel = root.Q<Label>("MeleeUpgradeLabel");
        DashLabel = root.Q<Label>("DashLabel");
        WallJumpLabel = root.Q<Label>("WallJumpLabel");
        WallBreakLabel = root.Q<Label>("WallBreakLabel");
        WarpLabel = root.Q<Label>("WarpLabel");
        DoubleJumpLabel = root.Q<Label>("DoubleJumpLabel");



        //devMenu Variables
        devMenu = root.Q<VisualElement>("devMenu");
        healButton = root.Q<Button>("healButton");
        wallJumpToggle = root.Q<Toggle>("wallJumpToggle");
        wallBreakToggle = root.Q<Toggle>("wallBreakToggle");
        dashToggle = root.Q<Toggle>("dashToggle");
        warpToggle = root.Q<Toggle>("warpToggle");
        meleeToggle = root.Q<Toggle>("meleeToggle");
        teleportDestination = root.Q<TextField>("tpDestinationField");
        teleportButton = root.Q<Button>("teleportButton");
        unlockRoomsButton = root.Q<Button>("unlockRoomsButton");


        wallJumpToggle.value = PlayerInfo.pInfo.hasWallJump;
        wallBreakToggle.value = PlayerInfo.pInfo.hasWallBreak;
        dashToggle.value = PlayerInfo.pInfo.hasDash;
        warpToggle.value = PlayerInfo.pInfo.hasWarp;
        meleeToggle.value = PlayerInfo.pInfo.hasMelee;
        

        healButton.clicked += HealButtonPressed;
        wallJumpToggle.RegisterValueChangedCallback(WallJumpTogglePressed);
        wallBreakToggle.RegisterValueChangedCallback(WallBreakTogglePressed);
        dashToggle.RegisterValueChangedCallback(DashTogglePressed);
        warpToggle.RegisterValueChangedCallback(WarpTogglePressed);
        meleeToggle.RegisterValueChangedCallback(MeleeTogglePressed);
        teleportButton.clicked += TeleportButtonPressed;
        unlockRoomsButton.clicked += UnlockRoomsButtonPressed;
    }

    private void Update() {
        //Every frame the game will check what menu the player is in.
        if (Input.GetKeyDown(KeyCode.Escape)) {
            //If the game is not already paused, set isPaused to true, pause the game, and set the active menu to the pauseMenu.
            //If the game is paused, reverse the above.
            if (!isPaused){
                isPaused=true;
                activeMenu = MenuState.PauseMenu;
                Time.timeScale = 0;
            } else {
                isPaused=false;
                activeMenu = MenuState.none;
                Time.timeScale = 1;
            }
        }

        updateMenu();
        updateHealthBar();


    }


    public void updateHealthBar(){
        hpBarCurrentWidth =((float)PlayerInfo.pInfo.currentHealth / (float)PlayerInfo.pInfo.maxHealth) * 100;
        root.Q<VisualElement>("hpFill").style.width = Length.Percent(hpBarCurrentWidth);
        root.Q<Label>("hpExact").text = "HP: " + PlayerInfo.pInfo.currentHealth + "/" + PlayerInfo.pInfo.maxHealth;
    }

    public void updateMenu(){
        switch (activeMenu){
            case MenuState.none:
                pauseMenu.style.display = DisplayStyle.None;
                //settingsMenu.style.display = DisplayStyle.None;
                root.Q<VisualElement>("devMenu").style.display = DisplayStyle.None;
                break;
            case MenuState.PauseMenu:
                pauseMenu.style.display = DisplayStyle.Flex;
                //settingsMenu.style.display = DisplayStyle.None;
                root.Q<VisualElement>("devMenu").style.display = DisplayStyle.None;
                updateItemList();
                break;
            case MenuState.SettingsMenu:
                pauseMenu.style.display = DisplayStyle.None;
                //settingsMenu.style.display = DisplayStyle.Flex;
                root.Q<VisualElement>("devMenu").style.display = DisplayStyle.None;
                break;
            case MenuState.devMenu:
                pauseMenu.style.display = DisplayStyle.None;
                //settingsMenu.style.display = DisplayStyle.None;
                root.Q<VisualElement>("devMenu").style.display = DisplayStyle.Flex;
                teleportDestination = root.Q<TextField>("tpDestinationField");
                break;
        }
    }

    public void updateItemList(){
        //Check if the player has the ability, then make the label visible to reflect that.
        if (PlayerInfo.pInfo.hasMelee){
            MeleeLabel.style.display = DisplayStyle.Flex;
        } else {
            MeleeLabel.style.display = DisplayStyle.None;
        }
        if (PlayerInfo.pInfo.hasMeleeUpgrade){
            MeleeUpgradeLabel.style.display = DisplayStyle.Flex;
        } else {
            MeleeUpgradeLabel.style.display = DisplayStyle.None;
        }
        if (PlayerInfo.pInfo.hasDash){
            DashLabel.style.display = DisplayStyle.Flex;
        } else {
            DashLabel.style.display = DisplayStyle.None;
        }
        if (PlayerInfo.pInfo.hasWallJump){
            WallJumpLabel.style.display = DisplayStyle.Flex;
        } else {
            WallJumpLabel.style.display = DisplayStyle.None;
        }
        if (PlayerInfo.pInfo.hasWallBreak){
            WallBreakLabel.style.display = DisplayStyle.Flex;
        } else {
            WallBreakLabel.style.display = DisplayStyle.None;
        }
        if (PlayerInfo.pInfo.hasWarp){
            WarpLabel.style.display = DisplayStyle.Flex;
        } else {
            WarpLabel.style.display = DisplayStyle.None;
        }
        if (PlayerInfo.pInfo.hasDoubleJump){
            DoubleJumpLabel.style.display = DisplayStyle.Flex;
        } else {
            DoubleJumpLabel.style.display = DisplayStyle.None;
        }


    }

    void ResumeButtonPressed(){
        isPaused=false;
        activeMenu = MenuState.none;
        Time.timeScale = 1;
    }

    //When the settings button is pressed 
    void SettingsButtonPressed(){
        Debug.Log("Settings button pressed.");
    }

    void MenuButtonPressed(){
        Debug.Log("Menu button pressed.");
        SceneManager.LoadScene("TitleScreen");
    }

    void SecretButtonPressed(){
        Debug.Log("Secret button pressed!");
        secretCount++;
        if (secretCount >= 3){
            secretCount = 0;
            Debug.Log("Open dev panel");
            activeMenu = MenuState.devMenu;
        }
    }

    void HealButtonPressed(){
        PlayerInfo.pInfo.currentHealth = PlayerInfo.pInfo.maxHealth;
    }

    void WallJumpTogglePressed(ChangeEvent<bool> evt){
        PlayerInfo.pInfo.hasWallJump = evt.newValue;
    }

    void WallBreakTogglePressed(ChangeEvent<bool> evt){
        PlayerInfo.pInfo.hasWallBreak = evt.newValue;
    }

    void DashTogglePressed(ChangeEvent<bool> evt){
        PlayerInfo.pInfo.hasDash = evt.newValue;
    }

    void WarpTogglePressed(ChangeEvent<bool> evt){
        PlayerInfo.pInfo.hasWarp = evt.newValue;
    }

    void MeleeTogglePressed(ChangeEvent<bool> evt){
        PlayerInfo.pInfo.hasMelee = evt.newValue;
    }

    void TeleportButtonPressed(){
        Debug.Log("Teleporting to " + teleportDestination.text);
        SceneManager.LoadScene(teleportDestination.text);
    }

    void UnlockRoomsButtonPressed(){
        Debug.Log("Unlocking all rooms");
        MapLoader.S.unlockMap();
    }



}
