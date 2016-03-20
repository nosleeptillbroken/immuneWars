// MainMenuScript.cs
// Defines main menu behaviour and actions.

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour 
{
	// Create references to Canvas and start and exit Buttons
	public GameObject quitMenu;
	public Button startButton;
	public Button exitButton;

	// Initialize menu and buttons
	void Start()
	{
		startButton = startButton.GetComponent<Button>();
		exitButton = exitButton.GetComponent<Button>();
	}

    public void StartGame()
    {
        StateManager.instance.SetState(StateManager.GameState.InGame, "Level 1");
    }

	// call when exit Button is pressed
	public void ExitGame()
	{
        // Enable the quit menu, disable the buttons
        quitMenu.gameObject.SetActive(true);
		startButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);
    }

    // when exit is pressed
    public void ConfirmExit()
    {
        Application.Quit();
    }

    // when nothing is pressed
    public void CancelExit()
	{
		// disable the menu, enable the buttons
        quitMenu.gameObject.SetActive(false);
		startButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(true);
    }


}