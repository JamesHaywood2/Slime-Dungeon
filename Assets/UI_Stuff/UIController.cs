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

        //devMenu Variables
        devMenu = root.Q<VisualElement>("devMenu");
        healButton = root.Q<Button>("healButton");
        wallJumpToggle = root.Q<Toggle>("wallJumpToggle");
        wallBreakToggle = root.Q<Toggle>("wallBreakToggle");
        dashToggle = root.Q<Toggle>("dashToggle");
        warpToggle = root.Q<Toggle>("warpToggle");
        meleeToggle = root.Q<Toggle>("meleeToggle");

        healButton.clicked += HealButtonPressed;
        wallJumpToggle.RegisterValueChangedCallback(WallJumpTogglePressed);
        wallBreakToggle.RegisterValueChangedCallback(WallBreakTogglePressed);
        dashToggle.RegisterValueChangedCallback(DashTogglePressed);
        warpToggle.RegisterValueChangedCallback(WarpTogglePressed);
        meleeToggle.RegisterValueChangedCallback(MeleeTogglePressed);
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
                break;
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




}
