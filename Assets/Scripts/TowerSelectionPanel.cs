using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TowerSelectionPanel : MonoBehaviour {

    public TowerSelector towerSelector = null;

    public TowerBehaviour selectedTower { get { return towerSelector.selectedTower; } }
    public TowerAttributes selectedAttributes { get { return towerSelector.selectedTower.attributes; } }

    private Text displayNameUI;
    private Dropdown targetingModeUI;

    /// <summary>
    /// Destroys the selected tower without selling it.
    /// </summary>
    public void DestroySelectedTower()
    {
        if(towerSelector && towerSelector.selectedTower)
        {
            Destroy(towerSelector.selectedTower.gameObject);
            towerSelector.DeselectTower();
        }
    }

    /// <summary>
    /// Sells the selected tower.
    /// </summary>
    public void SellSelectedTower()
    {
        if(towerSelector)
        {
            DestroySelectedTower();
        }
    }

    /// <summary>
    /// Updates the information displayed on the selection panel with the information from the tower and its attributes.
    /// </summary>
    public void UpdateDisplayInformation()
    {
        displayNameUI.text = selectedAttributes.displayName;
        targetingModeUI.value = (int)selectedTower.targetingMode;
    }

    /// <summary>
    /// Updates the information for the tower and its attributes using the information from the selection panel.
    /// </summary>
    public void UpdateTowerInformation()
    {
        selectedTower.targetingMode = (TowerBehaviour.TargetingMode)targetingModeUI.value;
    }

    //
    void Start()
    {
        displayNameUI = transform.FindChild("Name").GetComponent<Text>();
        targetingModeUI = transform.FindChild("Dropdown").GetComponent<Dropdown>();
    }

    //
    void Update()
    {

    }

}
