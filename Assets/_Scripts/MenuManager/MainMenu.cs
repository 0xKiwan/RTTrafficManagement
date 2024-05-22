using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

// This will be a component of the main menu canvas object.
public class MainMenu : MonoBehaviour
{
    public UnityEngine.UI.Button quitButton;    // The quit button.
    public UnityEngine.UI.Button optionsButton; // The options button.
    public UnityEngine.UI.Button resumeButton;  // The resume button.
    private Canvas optionsCanvas;               // The options canvas.
    public bool menuActive = false;             // Whether or not the menu is active.
    Canvas canvas;                              // The canvas object.

    // Handler for the quit button.
    public void QuitButtonHandler()
    {
        // Log that the user is quitting the game.
        Debug.Log("Quitting game...");

        // Quit the game.
        Application.Quit();
    }

    // Handler for the options button.
    public void OptionsButtonHandler()
    {
        // Log that the user is opening the options menu.
        Debug.Log("Opening options menu...");

        // Show the options canvas.
        optionsCanvas.enabled = true;
    }

    // Awake is called when the script instance is being loaded.
    void Awake()
    {
        // Get the canvas component.
        canvas = GetComponent<Canvas>();
        
        // Hide the canvas
        canvas.enabled = false;

        // Get the OptionsMenu canvas.
        optionsCanvas = GameObject.Find("OptionsMenu").GetComponent<Canvas>();

        // Hide the options canvas.
        optionsCanvas.enabled = false;
    }

    // Used to toggle the menu.
    public void ToggleMenu()
    {
        // Toggle the menu.
        menuActive = !menuActive;
        // Debug.Log("Menu active: " + menuActive.ToString() + ".");

        // Check if the menu is active, if so, show it.
        if (menuActive) canvas.enabled = true;

        // Otherwise, the menu is not active.
        else 
        {
            // Hide the options canvas.
            optionsCanvas.enabled = false;

            // Hide the main canvas
            canvas.enabled = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Add a listener to the quit button.
        quitButton.onClick.AddListener(QuitButtonHandler);

        // Add a listener to the options button.
        optionsButton.onClick.AddListener(OptionsButtonHandler);

        // Add a listener to the resume button.
        resumeButton.onClick.AddListener(ToggleMenu);
    }

    // Update is called once per frame
    void Update()
    {
        // Check if "ESC" is pressed.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Toggle the menu.
            ToggleMenu();
        }
    }
}
