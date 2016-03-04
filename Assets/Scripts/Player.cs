using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Player : MonoBehaviour
{
    //Create objects for the gameover panel, player object, camera object.
    private GameObject gameOver;
    private GameObject towerSpawnerOb;
    private GameObject cameraOb;

    //Create object for gameWin panel, the score UI, and the scoring variables that govern it.
    private GameObject gameWin;
    public Text scoreUI;
    private int score;
    private GameObject scoreScreenOb;

    //Create object for the kills UI, and the variables that govern it.
    public Text killsUI;
    private int totalKills;     //Number of enemies killed by the player.
    private int totalMissed;    //Number of enemies leaked, added with total kills for win condition.
    public int winningKills;    //Number of enemies for level, all must be destroyed to win.
    private bool winFlag;
    private bool lossFlag;

    //Create a slide object for the health bar, and the variables that govern it.
    public Slider healthBar;
    public int maxHealth = 50;
    public int currentHealth;

    void Start()
    {
        //Reference health bar, game over , and game win objects.
        healthBar = GameObject.Find("Health Bar").GetComponent<Slider>();
        gameOver = GameObject.Find("Game Over");
        gameOver.SetActive(false);      //Deactivate gameOver screen.
        gameWin = GameObject.Find("Game Win");
        gameWin.SetActive(false);      //Deactivate gameWin screen.
        scoreScreenOb = GameObject.Find("Score Screen");
        scoreScreenOb.SetActive(false);      //Deactivate score screen.

        lossFlag = false;
        winFlag = false;

        //Initialize the player's health bar, setting max and min values, as well as current health (full).
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.minValue = 0;

        //Reference the player game object to activate/deactivate tower placement systems.
        //Reference the camera to deactivate pan/zoom/rotate features on gameover/gamewin.
        towerSpawnerOb = GameObject.Find("TowerSpawner");
        cameraOb = GameObject.Find("Main Camera");

        //Initialize score and total kills to 0.
        score = 0;
        totalKills = 0;
        totalMissed = 0;
    }

    void Update()
    {
        healthBar.value = currentHealth;                    //Adjust player health when taking damage.
        scoreUI.text = "Score: " + score.ToString();        //Adjust the player's score killing enemies.
        killsUI.text = "Kills: " + totalKills.ToString();   //Adjust the player's total kills.

        if (currentHealth <= 0 && lossFlag == false)             //Game Over State.
        {
            gameOverState();
            lossFlag = true;
        }   

        if ((totalKills + totalMissed) >= winningKills && winFlag == false) //Game Win State.
        {
            gameWinState();
            winFlag = true;
        }
    }

    //Function adds to score when creep is killed.
    public void addScore()
    {
        score += 10;
        totalKills += 1;
    }

    //Function to add to missed kills when creep reaches despawn point.
    public void enemiesMissed()
    {
        totalMissed += 1;
    }

    //Function reloads current level, selected from gameover menu.
    public void reloadLevel()
    {
        //Reactivate camera controls once game is over.
        cameraOb.GetComponent<CameraController>().enableEdgePanning = true;
        cameraOb.GetComponent<CameraController>().rotationOn = true;
        cameraOb.GetComponent<CameraController>().zoomOn = true;

        towerSpawnerOb.SetActive(true);
        lossFlag = false;
        winFlag = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void TransitionToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    //Function displays score screen, selected from game win menu.
    public void scoreScreen()
    {
        scoreScreenOb.SetActive(true);      //Activate score screen.

        //More values can be added in the future for different creep types.
        int CKV = 10;   //Creep Kill Value
        int CKVtotal = CKV * totalKills;
        int CMV = -50;  //Creep Miss Value
        int CMVtotal = CMV * totalMissed;

        //Text for total creeps killed, and associated score.
        Text SSkills = GameObject.Find("KilledAmount").GetComponent<Text>();
        SSkills.text = totalKills + " x " + CKV + " = " + CKVtotal;
        //Text for total creeps killed, and associated deduction.
        Text SSmissed = GameObject.Find("MissedAmount").GetComponent<Text>();
        SSmissed.text = totalMissed + " x (" + CMV + ") = " + CMVtotal;
        //Total Score.
        Text SStotal = GameObject.Find("TotalAmount").GetComponent<Text>();
        SStotal.text = (CKVtotal + CMVtotal).ToString();
    }

    //Function handles what is activated/deactivated on game over.
    public void gameOverState()
    {
        Debug.Log("Game Lost");

        //Deactivate camera controls once game is over.
        cameraOb.GetComponent<CameraController>().enableEdgePanning = false;
        cameraOb.GetComponent<CameraController>().rotationOn = false;
        cameraOb.GetComponent<CameraController>().zoomOn = false;

        healthBar.transform.parent.gameObject.SetActive(false);     //Disable player health bar.
        gameOver.SetActive(true);                                   //Enable game over screen.

        towerSpawnerOb.SetActive(false);                            //Disable the tower ghost placement.
        GameObject.Find("Tower Ghost").SetActive(false);
    }

    //Function handles what is activated/deactivated on game win.
    public void gameWinState()
    {
        Debug.Log("Game Won");

        //Deactivate camera controls once game is over.
        cameraOb.GetComponent<CameraController>().enableEdgePanning = false;
        cameraOb.GetComponent<CameraController>().rotationOn = false;
        cameraOb.GetComponent<CameraController>().zoomOn = false;

        gameWin.SetActive(true);                                    //Enable game win screen.
        towerSpawnerOb.SetActive(false);                            //Disable the tower ghost placement.
        GameObject.Find("Tower Ghost").SetActive(false);
    }
}
