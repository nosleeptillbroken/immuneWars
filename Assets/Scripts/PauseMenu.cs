using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Experimental.Director;

public class PauseMenu : MonoBehaviour 
{
	//	Pause Menu Objects
	public GameObject pausePanel;
	public GameObject Camera;
	public GameObject quitConfirmPanel;



	//	Pause Menu Buttons
	public Button resume;
	public Button restart;
	public Button menu;

	bool paused = false;
	public PlayState state;




	void Start()
	{
		//	Set Pause Menu and Child Menus to Invisible upon start up
		pausePanel.SetActive(false);
		quitConfirmPanel.SetActive(false);
	}

	void Update()
	{
		//	If Statement to check if User has pressed the 'Esc' key to bring up and hide pause menu.
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			if(paused == true)		//	If the game is already paused, then unpause and resume play.
			{
				pausePanel.SetActive(false);
				paused = false;
				Time.timeScale = 1.0f;
			}
			else //		If the game is NOT paused, then pause the game.
			{
				pausePanel.SetActive(true);
				paused = true;
				Cursor.visible = true;
				Time.timeScale = 0.0f;
			}
		}
	}

	//	Alternative method to close pause panel
	public void resumeGame()
	{
		pausePanel.SetActive(false);
	}

	//	Restarts current level ******THIS IS THROWING A BIG ERROR. NEED TO ADDRESS THIS.
	public void restartLevel()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	//	Brings up confirmation Window asking user if they're sure they wish to quit.
	public void backToMain()
	{
		quitConfirmPanel.SetActive(true);
		pausePanel.SetActive(false);
	}

	//	Takes player back to the Main Menu
	public void goToMain()
	{
		SceneManager.LoadScene("Title Screen");
	}

	// Closes Prompt - Re-displays Pause Menu
	public void cancelQuit()
	{
		quitConfirmPanel.SetActive(false);
		pausePanel.SetActive(true);
	}

}
	