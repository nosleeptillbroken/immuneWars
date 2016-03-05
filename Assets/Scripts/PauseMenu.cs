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

    [Header("Interface Buttons")]
	//	Pause Menu Buttons
	public Button resume;
	public Button restart;
	public Button menu;

	private bool _paused = false;
    public bool paused { get { return _paused; } }

    private bool selectorPausedState = false;
    private bool spawnerPausedState = false;
    
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
                ResumeGame();
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
        spawnerPausedState = TowerSpawner.current.gameObject.activeInHierarchy;
        selectorPausedState = TowerSelector.current.gameObject.activeInHierarchy;

        TowerSpawner.current.gameObject.SetActive(false);
        TowerSelector.current.gameObject.SetActive(false);

        pausePanel.SetActive(true);
        _paused = true;
        Cursor.visible = true;
        Time.timeScale = 0.0f;
    }

	//	Alternative method to close pause panel
	public void ResumeGame()
    {

        TowerSpawner.current.gameObject.SetActive(spawnerPausedState);
        TowerSelector.current.gameObject.SetActive(selectorPausedState);

        pausePanel.SetActive(false);
        _paused = false;
        Time.timeScale = 1.0f;
    }

	//	Restarts current level ******THIS IS THROWING A BIG ERROR. NEED TO ADDRESS THIS.
	public void RestartLevel()
	{
        Player.current.ReloadCurrentLevel();
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

}
	