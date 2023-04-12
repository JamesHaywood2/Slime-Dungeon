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
    }

    private void StartButtonPressed()
    {
        Debug.Log("Start Button Pressed");
        SceneManager.LoadScene("Room_Start");
    }
}
