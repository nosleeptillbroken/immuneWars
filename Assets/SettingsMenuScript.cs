using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SettingsMenuScript : MonoBehaviour 
{
    //  Interface buttons
    public Button apply;

    //  Settings Panel
    public GameObject settingsMenuPanel;

    //  Settings Panel Components
    public Slider fxVol;
    public Slider musicVol;
    public Dropdown language;

	// Use this for initialization
	void Start () 
    {
        
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public void applyChanges()
    {
        settingsMenuPanel.SetActive(false);
    }
}
