using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public static UIController S;

    public Button startButton;

    VisualElement root;

    // Start is called before the first frame update
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        startButton = root.Q<Button>("startButton");
        startButton.clicked += StartButtonPressed;
        //When you start the title screen scene unload the hp bar in UIController.
        UIController.S.hpBarContainer.style.display = DisplayStyle.None;
    }

    private void StartButtonPressed()
    {
        Debug.Log("Start Button Pressed");
        //Turn on the hp bar in UIController.
        UIController.S.hpBarContainer.style.display = DisplayStyle.Flex;
        SceneManager.LoadScene(PlayerInfo.pInfo.lastRoom);
    }
}
