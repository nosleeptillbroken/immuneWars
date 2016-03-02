using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TowerSelectionPanel : MonoBehaviour {

    public TowerSelector towerSelector = null;

    public TowerBehaviour selectedTower { get { return towerSelector.selectedTower; } }

    private Text displayNameUI;
    private Dropdown targetingModeUI;

    public Button[] upgradeButtonsUI;

    /// <summary>
    /// Destroys the selected tower without selling it.
    /// </summary>
    public void DestroySelectedTower()
    {
        if(towerSelector && selectedTower)
        {
            Destroy(selectedTower.gameObject);
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

    public void UpgradeSelectedTower(int path)
    {
        if(towerSelector && selectedTower)
        {
            selectedTower.Upgrade(path);
        }
    }

    /// <summary>
    /// Updates the information displayed on the selection panel with the information from the tower and its attributes.
    /// </summary>
    public void UpdateDisplayInformation()
    {
        TowerAttributes compositeAttributes = selectedTower.attributes + selectedTower.upgradeAttributes;
        displayNameUI.text = compositeAttributes.displayName;
        targetingModeUI.value = (int)selectedTower.targetingMode;
        int i = 0;
        foreach (Button btn in upgradeButtonsUI)
        {
            if (selectedTower.CanUpgrade(i))
            {
                btn.GetComponentInChildren<Text>().text = selectedTower.GetNextUpgrade(i).displayName;
                btn.interactable = true;
            }
            else
            {
                btn.GetComponentInChildren<Text>().text = "Complete";
                btn.interactable = false;
            }
            i += 1;
        }
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
