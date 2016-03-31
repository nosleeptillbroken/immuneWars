using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Experimental.Director;

public class PauseMenu : MonoBehaviour 
{
    [Header("Interface Objects")]
	//	Pause Menu Objects
	public GameObject pausePanel;
	public GameObject quitConfirmPanel;
    public GameObject settingsMenuPanel;

    [Header("Interface Buttons")]
	//	Pause Menu Buttons
	public Button resume;
	public Button restart;
	public Button menu;
    public Button settings;

	private bool _paused = false;
    public bool paused { get { return _paused; } }

    private bool managerPausedState = false;
    
	void Start()
	{
		//	Set Pause Menu and Child Menus to Invisible upon start up
		pausePanel.SetActive(false);
		quitConfirmPanel.SetActive(false);
	}

	void Update()
	{
		//	If Statement to check if User has pressed the 'Esc' key to bring up and hide pause menu.
		if(Input.GetButtonDown("Pause"))
		{
			if(_paused == true)		//	If the game is already paused, then unpause and resume play.
			{
                if (settingsMenuPanel.activeInHierarchy)
                {
                    CloseSettings();
                }
                else
                {
                    ResumeGame();
                }
			}
			else //		If the game is NOT paused, then pause the game.
			{
                PauseGame();
			}
		}
	}

    //
    public void PauseGame()
    {
        managerPausedState = TowerManager.instance.gameObject.activeInHierarchy;

        TowerManager.instance.gameObject.SetActive(false);

        pausePanel.SetActive(true);
        _paused = true;
        Cursor.visible = true;
        Time.timeScale = 0.0f;
    }

	//	Alternative method to close pause panel
	public void ResumeGame()
    {
        TowerManager.instance.gameObject.SetActive(managerPausedState);

        pausePanel.SetActive(false);
        _paused = false;
        Time.timeScale = 1.0f;
    }

	//	Restarts current level ******THIS IS THROWING A BIG ERROR. NEED TO ADDRESS THIS.
	public void RestartLevel()
	{
        Player.instance.ReloadCurrentLevel();
	}

	//	Brings up confirmation Window asking user if they're sure they wish to quit.
	public void QuitGame()
	{
		quitConfirmPanel.SetActive(true);
		pausePanel.SetActive(false);
	}

	//	Takes player back to the Main Menu
	public void ConfirmQuit()
	{
		SceneManager.LoadScene("Title Screen");
	}

	// Closes Prompt - Re-displays Pause Menu
	public void CancelQuit()
	{
		quitConfirmPanel.SetActive(false);
		pausePanel.SetActive(true);
	}

    // Opens Settings Window
    public void OpenSettings()
    {
        settingsMenuPanel.SetActive(true);
    }

    // Closes Settings Window
    public void CloseSettings()
    {
        settingsMenuPanel.SetActive(false);
    }
}
	