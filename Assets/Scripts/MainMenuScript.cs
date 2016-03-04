// MainMenuScript.cs
// Defines main menu behaviour and actions.

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour 
{
	// Create references to Canvas and start and exit Buttons
	public Canvas quitMenu;
	public Button startButton;
	public Button exitButton;

	// Initialize menu and buttons
	void Start()
	{
		quitMenu = quitMenu.GetComponent<Canvas>();
		startButton = startButton.GetComponent<Button>();
		exitButton = exitButton.GetComponent<Button>();
		quitMenu.enabled = false; // disable menu at start
	}

	// call when exit Button is pressed
	public void exitPressed()
	{
		// Enable the quit menu, disable the buttons
		quitMenu.enabled = true; 
		startButton.enabled = false;
		exitButton.enabled = false;
	}

	// when nothing is pressed
	public void noPressed()
	{
		// disable the menu, enable the buttons
		quitMenu.enabled = false;
		startButton.enabled = true;
		exitButton.enabled = true;
	}
	
	/*
		Currently only loads "Test Level"
		Consider taking an string input parameter
		This way we can increase modularity and usefulness of script
	*/
	
	public void startLevel()
	{
		SceneManager.LoadScene("Test Level");
	}

	public void exitGame()
	{
		Application.Quit();
	}
}